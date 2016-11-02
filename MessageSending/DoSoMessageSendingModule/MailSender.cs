using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Reports;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.XtraPrinting;
using DoSo.Reporting.BusinessObjects;
using DoSo.Reporting.BusinessObjects.Email;
using DoSo.Reporting.Generators;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading;
using DevExpress.DashboardWin;
using NewBaseModule.BisinessObjects;

//using DevExpress.DataAccess.Native;
//using DevExpress.ExpressApp.Model.Core;
//using DevExpress.Utils.Design;
//using DevExpress.XtraSplashForm;
//using DevExpress.XtraSplashScreen;
//using DoSo.MessageSendingModule;
//using DoSo.Reporting.BusinessObjects.Base;

namespace DoSoMessageSendService
{
    public class MailSender
    {
        static string SmtpServer;
        static int SmtpPort;
        static bool SmtpEnableSsl;
        static bool UseDefaultCredentials;
        static string SmtpUserName;
        static string SmtpPassword;
        static int EmailSendingTimeOut;
        static string MailFrom;
        static bool executing;
        static bool ConfigurationWasTaken;

        public MailSender() { }
        public MailSender(string smtpServer, int smtpPort, bool enableSsl, bool useDefaultCredentials, string smtpUserName, string smtpPassword, string mailFrom)
        {
            SmtpServer = smtpServer;
            SmtpPort = smtpPort;
            SmtpEnableSsl = enableSsl;
            UseDefaultCredentials = useDefaultCredentials;
            SmtpUserName = smtpUserName;
            SmtpPassword = smtpPassword;
            MailFrom = mailFrom;
        }

        [STAThread]
        public void OnCallBack(Timer timer)
        {
            if (executing)
            {
                if (EmailSendingTimeOut == 0)
                    timer.Change(EmailSendingTimeOut, Timeout.Infinite);
                return;
            }

            executing = true;

            try
            {
                using (var unitOfWork = new UnitOfWork(new SimpleDataLayer(GeneratorHelper.GetConnection())))
                {
                    GetConfiguration(unitOfWork);

                    var mailsToSend = unitOfWork.Query<DoSoEmail>().Where(x =>
                                                                        !x.IsCanceled &&
                                                                        x.ExpiredOn == null &&
                                                                        !x.IsSent &&
                                                                        x.SendingDate < DateTime.Now);

                    foreach (var mail in mailsToSend)
                    {
                        try
                        {
                            var schedule = mail.DoSoReportSchedule;
                            if (schedule != null)
                            {
                                schedule = unitOfWork.GetObjectByKey<DoSoReportSchedule>(schedule.ID);
                                if (!schedule.IsActive)
                                    continue;
                            }

                            var reportData = mail.ReportData;

                            if (reportData == null && schedule?.ReportData != null)
                                reportData = schedule.ReportData;

                            ReportExportFileFormatEnum? reportType = null;
                            object obj = null;
                            if (reportData != null)
                            {
                                var type = GeneratorHelper.GetMyObjectType(mail.ObjectTypeName);
                                var classInfo = unitOfWork.GetClassInfo(type);
                                var key = classInfo.KeyProperty.Name;

                                obj = unitOfWork.FindObject(classInfo, CriteriaOperator.Parse($"{key} = {mail.ObjectKey}"));
                                reportType = mail.ExportFileFormat;

                                if (reportType == null)
                                    reportType = mail?.DoSoReportSchedule?.ExportFileFormat;
                            }

                            var isSent = SendMail(mail.EmailTo, mail.EmailCC, mail.SourceFilePath, mail.EmailSubject, mail.EmailBody, reportData, obj, reportType, schedule);

                            if (isSent)
                            {
                                if (!string.IsNullOrEmpty(mail.SourceFilePath))
                                {
                                    var splitedFilePaths = mail.SourceFilePath.Split(';');
                                    foreach (var splitedFilePath in splitedFilePaths)
                                    {
                                        if (splitedFilePath.Length < 10)
                                            continue;

                                        try
                                        {
                                            var folderName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SentFiles");
                                            if (!Directory.Exists(folderName))
                                                Directory.CreateDirectory(folderName);

                                            var newPath = splitedFilePath.Replace("GeneratedFiles", "SentFiles");
                                            File.Move(splitedFilePath, newPath);
                                            mail.TargetFilePath += newPath + ";";
                                        }
                                        catch (Exception) {/*Ignored*/}
                                    }
                                }

                                mail.SentDate = DateTime.Now;
                                mail.IsSent = true;
                                //mail.DoSoEmailSchedule.GetNextExecutionDate();
                                unitOfWork.CommitChanges();
                            }
                        }
                        catch (Exception ex)
                        {
                            GeneratorHelper.CreateLogFileWithException($"Exception On Foreach. ID = {mail.ID}{Environment.NewLine}{ex}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                executing = false;
                GeneratorHelper.CreateLogFileWithException(ex.ToString());
            }

            if (EmailSendingTimeOut > 0)
                timer.Change(EmailSendingTimeOut, Timeout.Infinite);

            executing = false;
        }

        public string AddDashboardToMail(MailMessage mail, DashboardDefinition dashboard, DoSoDoSoEmailSchedule schedule)
        {
            try
            {
                var template = dashboard.CreateDashBoard();
                var fileName = DateTime.Now.ToString("yyyMMddHHmmssfff");
                var tempPath = Path.GetTempPath();
                var path = Path.Combine(tempPath, fileName);

                using (var viewwer = new DashboardViewer())
                {
                    var width = Convert.ToInt32(schedule?.DashboardWidth.IsNull(1600));
                    var height = Convert.ToInt32(schedule?.DashboardHeight.IsNull(1000));
                    var fileType = schedule?.ExportFileFormat;

                    viewwer.Size = new Size(width, height);
                    viewwer.Dashboard = template;

                    if (fileType == ReportExportFileFormatEnum.HTML)
                    {
                        path += ".png";
                        viewwer.ExportToImage(path);
                        mail.IsBodyHtml = true;

                        var inlineLogo = new Attachment(path);
                        mail.Attachments.Add(inlineLogo);
                        inlineLogo.ContentId = fileName;
                        mail.Body += Environment.NewLine + "<htm><body> <img src=\"cid:" + fileName + "\"> </body></html>";
                        Console.WriteLine("Body Assigned");

                        return path;
                    }

                    if (fileType == ReportExportFileFormatEnum.PDF)
                    {
                        path += ".pdf";
                        viewwer.ExportToPdf(path);
                        var attachment = new Attachment(path);
                        mail.Attachments.Add(attachment);
                        return path;
                    }
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                GeneratorHelper.CreateLogFileWithException(ex.ToString());
                return "";
            }
        }

        //private void Viewwer_ConnectionError(object sender, DevExpress.DataAccess.ConnectionErrorEventArgs e)
        //{
        //    GeneratorHelper.CreateLogFileWithException(Environment.UserInteractive + Environment.NewLine + e.Exception);
        //}

        public bool SendMail(string mailTo, string EmailCC, string path, string subject, string body, ReportData report, object obj, ReportExportFileFormatEnum? fileType, DoSoReportSchedule schedule = null)
        {
            //if (report == null)
            //    return false;
            using (var mail = new MailMessage())
            {

                //var dashboard = schedule?.DashboardDefinition;

                var otherFilePath = string.Empty;
                try
                {
                    if (report != null && obj != null)
                        otherFilePath = AddAlternateViewWithLinkedResource(mail, report, obj, fileType);

                    if (!string.IsNullOrEmpty(path))
                    {
                        var paths = path.Split(';');
                        foreach (var s in paths)
                        {
                            if (s.Length < 10)
                                continue;
                            var attachment = new Attachment(s);
                            mail.Attachments.Add(attachment);
                        }
                    }

                    mail.From = new MailAddress(MailFrom);

                    var tos = mailTo.Split(';');
                    foreach (var item in tos.Where(x => x.Length > 2))
                        mail.To.Add(new MailAddress(item.Trim()));

                    if (!string.IsNullOrEmpty(EmailCC))
                    {
                        var ccs = EmailCC.Split(';');
                        foreach (var item in ccs.Where(x => x.Length > 2))
                            mail.CC.Add(new MailAddress(item.Trim()));
                    }

                    mail.Subject = subject;
                    mail.Body += body;

                    //if (dashboard != null)
                    //    try
                    //    {
                    //        otherFilePath = AddDashboardToMail(mail, dashboard, schedule);
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        throw new InvalidOperationException("Cannot Generate Dashboard" + Environment.NewLine + ex);
                    //    }


                    using (var client = new SmtpClient()
                    {
                        Host = SmtpServer,
                        Port = SmtpPort,
                        EnableSsl = SmtpEnableSsl,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = UseDefaultCredentials,
                        Credentials = new NetworkCredential(SmtpUserName, SmtpPassword),
                        Timeout = 50000
                    })
                    {
                        client.Send(mail);
                    }

                    DisposeResources(mail);
                    DeleteSentFiles(otherFilePath);

                    return true;
                }
                catch (Exception ex)
                {
                    //if (!string.IsNullOrEmpty(otherFilePath))
                    //     File.Delete(otherFilePath); 
                    DisposeResources(mail);
                    GeneratorHelper.CreateLogFileWithException(ex.ToString());
                    return false;
                }
            }
        }

        public void DeleteSentFiles(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    if (Directory.Exists(path))
                        Directory.Delete(path);
                    else
                        File.Delete(path);
                }
                catch (Exception) {/*Ignored*/}
            }
        }

        public void DisposeResources(MailMessage mail)
        {
            foreach (var item in mail.Attachments)
                item.Dispose();

            try
            {
                foreach (var item in mail.AlternateViews.SelectMany(x => x.LinkedResources))
                    item.Dispose();
            }
            catch (Exception) { }

            foreach (var item in mail.AlternateViews)
                item.Dispose();
        }

        public static XafReport LoadXafReport(ReportData report, object obj)
        {
            var connectionString = GeneratorHelper.GetConnectionString();
            //using (var connection = new SqlConnection(connectionString))
            //{
            var objectSpaceProvider = new XPObjectSpaceProvider(connectionString, XpoDefault.DataLayer.Connection);
            var objectSpace = objectSpaceProvider.CreateObjectSpace() as XPObjectSpace;

            var type = GeneratorHelper.GetMyObjectType(report.DataTypeName);
            var classInfo = objectSpace?.Session.GetClassInfo(type);
            var key = classInfo?.KeyProperty;

            XafTypesInfo.Instance.RegisterEntity(type);
            var xafReport = report.LoadReport(objectSpace) as XafReport;
            xafReport.Filtering.Filter = string.Format("{0}={1}", key.Name, key.GetValue(obj));
            xafReport.ApplyFiltering();
            return xafReport;
            //}
        }

        static string GetHtmlContent(ReportData report, object obj, string path, string fileName)
        {
            var xafReport = LoadXafReport(report, obj);
            xafReport.ExportToHtml(path + fileName, new HtmlExportOptions("UTF-8"));

            var strHTML = File.ReadAllText(path + fileName);
            strHTML = strHTML.Replace(fileName + "_files/", "");
            strHTML = strHTML.Replace("src=\"", "src=cid:");
            strHTML = strHTML.Replace("gif\"", "gif");
            strHTML = strHTML.Replace("png\"", "png");

            return strHTML;
        }


        static string AddAlternateViewWithLinkedResource(MailMessage mail, ReportData report, object obj, ReportExportFileFormatEnum? fileType)
        {
            var path = Path.GetTempPath();
            var fileName = DateTime.Now.ToString("yyyMMddHHmmssfff");

            if (fileType == ReportExportFileFormatEnum.HTML)
            {
                var strHTML = GetHtmlContent(report, obj, path, fileName);
                //using (var avHtml = AlternateView.CreateAlternateViewFromString(strHTML, Encoding.Unicode, MediaTypeNames.Text.Html))
                //{

                var avHtml = AlternateView.CreateAlternateViewFromString(strHTML, Encoding.Unicode, MediaTypeNames.Text.Html);
                mail.AlternateViews.Add(avHtml);

                var di = new DirectoryInfo(Path.Combine(path, fileName + "_files"));
                di.Create();
                var files = di.GetFiles("*.*");

                foreach (var item in files)
                {
                    var fileMimeType = GetMimeType(item.Name);
                    //using (var pic1 = new LinkedResource(String.Format("{0}\\{1}", di.FullName, item.Name), new ContentType(fileMimeType)))
                    //{
                    var pic1 = new LinkedResource($"{di.FullName}\\{item.Name}", new ContentType(fileMimeType)) { ContentId = item.Name };
                    avHtml.LinkedResources.Add(pic1);
                    //}
                }
                return Path.Combine(path, fileName + "_files");
                //}
            }

            if (fileType == ReportExportFileFormatEnum.PDF)
            {
                var _path = Path.Combine(path, fileName + ".Pdf");
                var xafReport = LoadXafReport(report, obj);
                xafReport.ExportToPdf(_path);

                //using ()
                var attachment = new Attachment(_path);
                //{
                mail.Attachments.Add(attachment);
                return _path;
                //}
            }

            if (fileType == ReportExportFileFormatEnum.Xlsx)
            {
                var _path = Path.Combine(path, fileName + ".xlsx");
                var xafReport = LoadXafReport(report, obj);
                xafReport.ExportToPdf(_path);
                using (var attachment = new Attachment(_path))
                {
                    mail.Attachments.Add(attachment);
                    return _path;
                }
            }
            return "";
        }

        static string GetMimeType(string fileName)
        {
            var mimeType = "application/unknown";
            var ext = Path.GetExtension(fileName)?.ToLower();
            var regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }

        static void GetConfiguration(UnitOfWork unitOfWork)
        {
            if (ConfigurationWasTaken)
                return;
            try
            {
                SmtpServer = ConfigurationStatic.GetParameterValue(@"DoSoEmailSmtpServer", unitOfWork);
                SmtpPort = Convert.ToInt32(ConfigurationStatic.GetParameterValue(@"DoSoEmailSmtpPort", unitOfWork));
                SmtpEnableSsl = Convert.ToBoolean(ConfigurationStatic.GetParameterValue(@"DoSoEmailEnableSslr", unitOfWork));
                UseDefaultCredentials = Convert.ToBoolean(ConfigurationStatic.GetParameterValue(@"DoSoEmailUseDefaultCredentials", unitOfWork));
                SmtpUserName = ConfigurationStatic.GetParameterValue(@"DoSoEmailSmtpUserName", unitOfWork);
                SmtpPassword = ConfigurationStatic.GetParameterValue(@"DoSoEmailSmtpPassword", unitOfWork);
                MailFrom = ConfigurationStatic.GetParameterValue(@"DoSoEmailMailFrom", unitOfWork);
                EmailSendingTimeOut = Convert.ToInt32(ConfigurationStatic.GetParameterValue(@"DoSoEmailSendingTimeOut", unitOfWork));

                ConfigurationWasTaken = true;
            }
            catch (Exception ex)
            {
                GeneratorHelper.CreateLogFileWithException(ex.ToString());
            }
        }
    }
}