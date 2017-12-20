using DevExpress.Xpo;
using DoSo.Reporting.BusinessObjects;
using DoSo.Reporting.BusinessObjects.Email;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static DoSo.Reporting.BusinessObjects.Base.DoSoMessageBase;

namespace DoSo.Reporting.Senders
{
    public static class MailSender
    {
        static object _locker = new object();
        public static void SendAll()
        {
            lock (_locker)
                try
                {
                    HS.GetOrCreateSericeStatus(nameof(MailSender));
                    using (var unitOfWork = new UnitOfWork(XpoDefault.DataLayer))
                    {
                        var allMessage2Send = unitOfWork.Query<DoSoEmail>().Where(x => x.Status == BusinessObjects.Base.DoSoMessageBase.MessageStatusEnum.Active && x.ExpiredOn == null && x.SendingDate < DateTime.Now && (x.DoSoReportSchedule == null || x.DoSoReportSchedule.IsActive));

                        foreach (var message2Send in allMessage2Send)
                        {
                            if (!HS.EnableMailSender)
                                return;
                            Send(message2Send, unitOfWork);
                            unitOfWork.CommitChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    HS.GetOrCreateSericeStatus(nameof(MailSender), true);
                    HS.CreateExceptionLog(ex.Message, ex.ToString(), 6);
                }
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

        public static void Send(DoSoEmail email, UnitOfWork unitOfWork)
        {
            try
            {
                using (var mail = new MailMessage())
                {


                    try
                    {
                        var fullPath = Path.Combine(email.FolderPath, "HtmlContent");
                        var strHTML = File.ReadAllText(fullPath);
                        strHTML = strHTML.Replace("HtmlContent" + "_files/", "");
                        strHTML = strHTML.Replace("src=\"", "src=cid:");
                        strHTML = strHTML.Replace("gif\"", "gif");
                        strHTML = strHTML.Replace("png\"", "png");
                        strHTML = strHTML.Replace("jpg\"", "jpg");

                        var avHtml = AlternateView.CreateAlternateViewFromString(strHTML, Encoding.Unicode, MediaTypeNames.Text.Html);
                        mail.AlternateViews.Add(avHtml);

                        var di = new DirectoryInfo(fullPath + "_files");
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
                    }
                    catch (Exception) { }


                    if (!string.IsNullOrEmpty(email.SourceFilePath))
                    {
                        var paths = email.SourceFilePath.Split(';');
                        foreach (var s in paths)
                        {
                            if (s.Length < 10)
                                continue;
                            var attachment = new Attachment(s);
                            mail.Attachments.Add(attachment);
                        }
                    }

                    mail.From = new MailAddress(HS.EMailFrom);

                    var tos = email.EmailTo.Split(';');
                    foreach (var item in tos.Where(x => x.Length > 2))
                        mail.To.Add(new MailAddress(item.Trim()));

                    var ccs = email?.EmailCC?.Split(';');
                    if (ccs != null)
                        foreach (var item in ccs?.Where(x => x.Length > 2))
                            mail.CC.Add(new MailAddress(item.Trim()));

                    mail.Subject = email.EmailSubject;
                    mail.Body += email.EmailBody;

                    mail.IsBodyHtml = true;

                    var inlineLogo = new Attachment(Path.Combine(email.FolderPath, "Dashboard.png"));
                    mail.Attachments.Add(inlineLogo);
                    inlineLogo.ContentId = "Dashboard";
                    mail.Body += Environment.NewLine + "<htm><body> <img src=\"cid:" + "Dashboard" + "\"> </body></html>";

                    using (var client = new SmtpClient(HS.SmtpServer, HS.SmtpPort))
                    {
                        client.EnableSsl = HS.EnableSsl;
                        //DeliveryMethod = SmtpDeliveryMethod.Network,
                        client.UseDefaultCredentials = HS.UseDefaultCredentials;
                        client.Credentials = new NetworkCredential(HS.SmtpUserName, HS.SmtpPassword);
                        client.Timeout = 50000;
                        client.Send(mail);
                    }

                    email.Status = BusinessObjects.Base.DoSoMessageBase.MessageStatusEnum.Sent;
                    email.SentDate = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                email.CancelMessage($"Exception Was Thrown. See Exception log ({DateTime.Now})", MessageStatusEnum.CancelledByService);
                HS.CreateExceptionLog(ex.Message, ex.ToString(), 5);
            }
        }
    }
}
