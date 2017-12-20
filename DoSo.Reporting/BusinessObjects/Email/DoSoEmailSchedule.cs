
using DevExpress.DashboardWin;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Model;
using DevExpress.LookAndFeel;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Spreadsheet;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.XtraReports.UI;
using DevExpress.XtraSpreadsheet;
using DoSo.Reporting.BusinessObjects.Base;
using DoSo.Reporting.BusinessObjects.Email;
using DoSo.Reporting.BusinessObjects.Reporting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace DoSo.Reporting.BusinessObjects
{
    [DefaultClassOptions]
    //[NavigationItem("Reports")]
    public class DoSoReportSchedule : DoSoScheduleBase
    {
        public DoSoReportSchedule(Session session) : base(session) { }

        string fMessageTo;
        [ElementTypeProperty("TargetObjectType")]
        [ModelDefault("PropertyEditorType", "DoSo.Reporting.Controllers.PopupExpressionPropertyEditorEx")]
        [Size(SizeAttribute.Unlimited)]
        public string MessageTo
        {
            get { return fMessageTo; }
            set { SetPropertyValue(nameof(MessageTo), ref fMessageTo, value); }
        }

        string fMessageCC;
        [Size(SizeAttribute.Unlimited)]
        [ElementTypeProperty("TargetObjectType")]
        [ModelDefault("PropertyEditorType", "DoSo.Reporting.Controllers.PopupExpressionPropertyEditorEx")]
        public string MessageCC
        {
            get { return fMessageCC; }
            set { SetPropertyValue(nameof(MessageCC), ref fMessageCC, value); }
        }

        string fMessageSubject;
        [Size(SizeAttribute.Unlimited)]
        [ElementTypeProperty("TargetObjectType")]
        [ModelDefault("PropertyEditorType", "DoSo.Reporting.Controllers.PopupExpressionPropertyEditorEx")]
        public string MessageSubject
        {
            get { return fMessageSubject; }
            set { SetPropertyValue(nameof(MessageSubject), ref fMessageSubject, value); }
        }

        string fAttachedFilesExpression;
        [Size(SizeAttribute.Unlimited)]
        [ElementTypeProperty("TargetObjectType")]
        [ModelDefault("PropertyEditorType", "DoSo.Reporting.Controllers.PopupExpressionPropertyEditorEx")]
        public string AttachedFilesExpression
        {
            get { return fAttachedFilesExpression; }
            set { SetPropertyValue(nameof(AttachedFilesExpression), ref fAttachedFilesExpression, value); }
        }

        string fMessageBody;
        [Size(SizeAttribute.Unlimited)]
        [ElementTypeProperty("TargetObjectType")]
        [ModelDefault("PropertyEditorType", "DoSo.Reporting.Controllers.PopupExpressionPropertyEditorEx")]
        public string MessageBody
        {
            get { return fMessageBody; }
            set { SetPropertyValue(nameof(MessageBody), ref fMessageBody, value); }
        }

        string fReportDataXml;
        [Size(SizeAttribute.Unlimited)]
        public string ReportDataXml
        {
            get { return fReportDataXml; }
            set { SetPropertyValue(nameof(ReportDataXml), ref fReportDataXml, value); }
        }

        //string fDashboardXml;
        //[Size(SizeAttribute.Unlimited)]
        //public string DashboardXml
        //{
        //    get { return fDashboardXml; }
        //    set { SetPropertyValue(nameof(DashboardXml), ref fDashboardXml, value); }
        //}


        //string fSheetReportXml;
        //[Size(SizeAttribute.Unlimited)]
        //public string SheetReportXml
        //{
        //    get { return fSheetReportXml; }
        //    set { SetPropertyValue(nameof(SheetReportXml), ref fSheetReportXml, value); }
        //}

        ReportData fReportData;
        public ReportData ReportData
        {
            get { return fReportData; }
            set { SetPropertyValue(nameof(ReportData), ref fReportData, value); }
        }

        DoSoDashboard fDashboard;
        public DoSoDashboard Dashboard
        {
            get { return fDashboard; }
            set { SetPropertyValue(nameof(Dashboard), ref fDashboard, value); }
        }

        DoSoReport fDoSoReport;
        public DoSoReport Report
        {
            get { return fDoSoReport; }
            set { SetPropertyValue(nameof(DoSoReport), ref fDoSoReport, value); }
        }

        ReportDataV2 fReportDataV2;
        public ReportDataV2 ReportDataV2
        {
            get { return fReportDataV2; }
            set { SetPropertyValue(nameof(ReportDataV2), ref fReportDataV2, value); }
        }

        //DashboardDefinition fDashboardDefinition;
        //public DashboardDefinition DashboardDefinition
        //{
        //    get { return fDashboardDefinition; }
        //    set { SetPropertyValue(nameof(DashboardDefinition), ref fDashboardDefinition, value); }
        //}


        int? fDashboardHeight;
        public int? DashboardHeight
        {
            get { return fDashboardHeight; }
            set { SetPropertyValue(nameof(DashboardHeight), ref fDashboardHeight, value); }
        }

        int? fDashboardWidth;
        public int? DashboardWidth
        {
            get { return fDashboardWidth; }
            set { SetPropertyValue(nameof(DashboardWidth), ref fDashboardWidth, value); }
        }


        ReportExportFileFormatEnum fExportFileFormat;
        public ReportExportFileFormatEnum ExportFileFormat
        {
            get { return fExportFileFormat; }
            set { SetPropertyValue(nameof(ExportFileFormat), ref fExportFileFormat, value); }
        }

        //List<ReportData> FilteredReports
        //{
        //    get { return Session.Query<ReportData>().Where(rd => rd.DataTypeName == BusinessObject.FullName).ToList(); }
        //}

        string fAlternativeText;
        [Size(SizeAttribute.Unlimited)]
        //[ElementTypeProperty("TargetObjectType")]
        public string AlternativeText
        {
            get { return fAlternativeText; }
            set { SetPropertyValue(nameof(AlternativeText), ref fAlternativeText, value); }
        }

        //ReportDefinition fReportDefinition;
        //[Association("ReportDefinition-Schedule")]
        //public ReportDefinition ReportDefinition
        //{
        //    get { return fReportDefinition; }
        //    set { SetPropertyValue(nameof(ReportDefinition), ref fReportDefinition, value); }
        //}

        [Association("DoSoEmailSchedule-QueryParameter")]
        public XPCollection<ScheduleQueryParameter> QueryParametersCollection => GetCollection<ScheduleQueryParameter>(nameof(QueryParametersCollection));

        [Association("DoSoEmailSchedule-DoSoEmail")]
        public XPCollection<DoSoEmail> DoSoEmailsCollection => GetCollection<DoSoEmail>(nameof(DoSoEmailsCollection));

        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);

            //if (propertyName == nameof(ReportDefinition) && ReportDefinition != null)
            //{
            //    QueryParametersCollection.ToList().Clear();

            //    //foreach (var parameter in ReportDefinition.QueryParametersCollection.Where(x => x.ExpiredOn == null))
            //    //{
            //    //    var newParameter = new ScheduleQueryParameter(Session) { DoSoReportSchedule = this };
            //    //    var properties = (parameter as QueryParameter).ClassInfo.PersistentProperties.OfType<ReflectionPropertyInfo>().Where(x => !x.IsKey);
            //    //    foreach (var property in properties)
            //    //    {
            //    //        var propertyValue = parameter.GetMemberValue(property.Name);
            //    //        if (propertyValue == ReportDefinition || property.MemberType == this.GetType())
            //    //            continue;

            //    //        newParameter.SetMemberValue(property.Name, propertyValue);
            //    //    }
            //    //}
            //}
        }

        public void AddDataSource2Report(XtraReport report)
        {
            if (ExcelDataSource != null)
                report.DataSource = ExcelDataSource;
            if (SqlDataSource != null)
                report.DataSource = SqlDataSource?.Result[0];
        }

        public override List<DoSoMessageBase> GenerateMessages(Session session, bool prevewOnly = false)
        {
            base.GenerateMessages(session);
            //var itemsList = GetObjectsFromDataSource();
            var item = this;
            var properties = session.GetProperties(/*itemsList.FirstOrDefault()*/item.ClassInfo);
            var emails = new List<DoSoMessageBase>();


            //foreach (var item in itemsList)
            {
                var email = new DoSoEmail(session)
                {
                    EmailTo = new ExpressionEvaluator(properties, MessageTo)?.Evaluate(item)?.ToString(),
                    EmailCC = new ExpressionEvaluator(properties, MessageCC)?.Evaluate(item)?.ToString(),
                    EmailSubject = new ExpressionEvaluator(properties, MessageSubject)?.Evaluate(item)?.ToString(),
                    EmailBody = new ExpressionEvaluator(properties, MessageBody)?.Evaluate(item)?.ToString(),
                    ObjectKey = new ExpressionEvaluator(properties, ObjectKeyExpression)?.Evaluate(item)?.ToString(),
                    ObjectTypeName = new ExpressionEvaluator(properties, ObjectTypeExpression)?.Evaluate(item)?.ToString(),
                    SendingDate = Convert.ToDateTime(new ExpressionEvaluator(properties, SendingDateExpression)?.Evaluate(item)),
                    ExportFileFormat = ExportFileFormat,
                    DoSoReportSchedule = session.GetObjectByKey<DoSoReportSchedule>(ID)
                };

                if (SkipExecutionDate != null)
                {
                    var sameEmail = DoSoEmailsCollection.Where(x => x.ExpiredOn == null && x.Status == DoSoMessageBase.MessageStatusEnum.Sent && email.SendingDate.AddDays(SkipExecutionDate ?? 0) > x.SentDate && x.EmailTo == email.EmailTo);
                    if (sameEmail.Any())
                    {
                        email.CancelMessage("Skipped", DoSoMessageBase.MessageStatusEnum.Skipped);
                        email.Delete();
                    }
                }

                if (!prevewOnly)
                {
                    var folderPath = Path.Combine(HS.GeteratedFilesName, HS.MyTempName);
                    email.FolderPath = folderPath;
                    Directory.CreateDirectory(folderPath);

                    GenerateExcelReport(email, item, session, folderPath);
                    GenerateReportData(email, false);
                    GenerateDashboard(email);
                }

                emails.Add(email);
            }

            return emails;
        }

        public void GenerateDashboard(DoSoEmail email, bool prevewOnly = false)
        {
            var dashboardXml = Dashboard?.Xml;
            if (string.IsNullOrEmpty(dashboardXml))
                return;

            var width = DashboardWidth ?? 1000;
            var height = DashboardHeight ?? 1000;

            using (var viewver = new DashboardViewer())
            {
                viewver.Dock = System.Windows.Forms.DockStyle.Fill;
                using (var ms = new MemoryStream())
                {
                    using (var sr = new StreamWriter(ms, Encoding.Default))
                    {
                        var doc = new XmlDocument();
                        doc.LoadXml(dashboardXml);
                        var definitionXml = doc.OuterXml;
                        sr.Write(definitionXml);
                        sr.Flush();
                        ms.Position = 0;
                        try
                        {
                            viewver.LoadDashboard(ms);
                        }
                        catch (Exception e)
                        {

                            throw;
                        }
                        
                        viewver.Size = new System.Drawing.Size(width, height);

                        if (!prevewOnly)
                        {
                            var path = Path.Combine(email.FolderPath, "Dashboard" + ".png");
                            viewver.ExportToImage(path);
                            //email.SourceFilePath += path + ";";
                        }
                        else
                        {
                            using (var form = new DevExpress.XtraEditors.XtraForm())
                            {
                                form.Width = width;
                                form.Height = height;
                                viewver.Parent = form;
                                form.ShowDialog();
                            }
                        }
                    }
                }
            }
        }

        public void GenerateReportData(DoSoEmail email, bool prevewOnly)
        {
            if (string.IsNullOrWhiteSpace(ReportDataXml))
                return;

            CreateDataSourceFromXml();
            ExcelDataSource?.Fill();
            SqlDataSource?.Fill();

            using (var report = new XtraReport())
            {
                AddDataSource2Report(report);

                using (var ms = new MemoryStream())
                {
                    using (var sr = new StreamWriter(ms, Encoding.Default))
                    {
                        var doc = new XmlDocument();
                        doc.LoadXml(ReportDataXml);
                        var definitionXml = doc.OuterXml;
                        sr.Write(definitionXml);
                        sr.Flush();
                        ms.Position = 0;
                        report.LoadLayoutFromXml(ms);
                        report.FilterString = $"{ObjectKeyExpression} == {email.ObjectKey}";
                        report.ApplyFiltering();
                        //report.FillDataSource();

                        if (prevewOnly)
                        {
                            using (ReportPrintTool printTool = new ReportPrintTool(report))
                                printTool.ShowRibbonPreviewDialog(UserLookAndFeel.Default);
                        }
                        else
                        {
                            ExportRportData(email, report);
                        }
                    }
                }
            }
        }


        public void ExportRportData(DoSoEmail email, XtraReport report)
        {
            if (email.ExportFileFormat == ReportExportFileFormatEnum.HTML)
                report.ExportToHtml(Path.Combine(email.FolderPath, "HtmlContent"));
            if (email.ExportFileFormat == ReportExportFileFormatEnum.PDF)
            {
                var filePath = Path.Combine(email.FolderPath, HS.MyTempName + ".Pdf");
                report.ExportToPdf(filePath);
                email.SourceFilePath += filePath + ";";
            }
            if (email.ExportFileFormat == ReportExportFileFormatEnum.Xlsx)
            {
                var filePath = Path.Combine(email.FolderPath, HS.MyTempName + ".Xlsx");
                report.ExportToXlsx(filePath);
                email.SourceFilePath += filePath + ";";
            }
        }

        public void GenerateExcelReport(DoSoEmail email, object item, Session session, string folderPath)
        {
            var xml = Report?.Xml;
            if (!string.IsNullOrEmpty(xml))
            {
                var control = new SpreadsheetControl();
                using (var ms = new MemoryStream(Convert.FromBase64String(xml)))
                    control.LoadDocument(ms, DocumentFormat.OpenXml);

                if (control.Document.MailMergeParameters.Any())
                    throw new InvalidOperationException("Need Parameters");

                var docs = control.Document.GenerateMailMergeDocuments();
                foreach (var doc in docs)
                {
                    var fullName = Path.Combine(folderPath, HS.MyTempName + ".Xlsx");
                    doc.SaveDocument(fullName);
                    email.SourceFilePath += fullName + ";";
                }
            }


            //if (ReportDefinition != null)
            //{
            //    var fullName = Path.Combine(folderPath, HS.MyTempName + ".Xlsx");
            //    SetParameteValues(this, item, session);
            //    var newFileaName = FastExportingMethod.ExportData2Excel(ReportDefinition, session, fullName);

            //    if (!string.IsNullOrEmpty(newFileaName))
            //        email.SourceFilePath += newFileaName + ";";
            //    else
            //        email.EmailBody += string.Format("{0}{0}{1}", Environment.NewLine, AlternativeText);
            //}
        }
        static void SetParameteValues(DoSoReportSchedule schedule, object item, Session unitOfWork)
        {
            //foreach (var parameter in schedule.QueryParametersCollection.Where(x => x.ExpiredOn == null))
            //{
            //    var parameterFromReport = schedule.ReportDefinition.QueryParametersCollection.Where(x => x.ExpiredOn == null && x.ParameterName == parameter.ParameterName);
            //    var parameterValue = new ExpressionEvaluator(unitOfWork.GetProperties(unitOfWork.GetClassInfo(item)), parameter.ParameterValueExression).Evaluate(item);
            //    parameterFromReport.FirstOrDefault().ParameterValue = parameterValue?.ToString();
            //}
        }
    }


    public enum ReportExportFileFormatEnum
    {
        HTML = 0,
        PDF = 1,
        Xlsx = 2
    }
}
