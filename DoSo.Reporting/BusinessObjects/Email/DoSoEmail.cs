using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DoSo.Reporting.BusinessObjects.Base;
using System.ComponentModel;
using System.Linq;

namespace DoSo.Reporting.BusinessObjects.Email
{
    //[DefaultClassOptions]
    [NavigationItem("Reports")]
    [DefaultProperty("EmailSubject")]
    [CreatableItem(false)]
    public class DoSoEmail : DoSoMessageBase
    {
        public DoSoEmail(Session session) : base(session) { }

        private DoSoReportSchedule fDoSoReportSchedule;
        [Association("DoSoEmailSchedule-DoSoEmail")]
        public DoSoReportSchedule DoSoReportSchedule
        {
            get { return fDoSoReportSchedule; }
            set { SetPropertyValue(nameof(DoSoReportSchedule), ref fDoSoReportSchedule, value); }
        }

        private string fFolderPath;
        public string FolderPath
        {
            get { return fFolderPath; }
            set { SetPropertyValue(nameof(FolderPath), ref fFolderPath, value); }
        }

        private string fEmailTo;
        [Size(SizeAttribute.Unlimited)]
        public string EmailTo
        {
            get { return fEmailTo; }
            set { SetPropertyValue(nameof(EmailTo), ref fEmailTo, value); }
        }

        private string fEmailCC;
        [Size(SizeAttribute.Unlimited)]
        public string EmailCC
        {
            get { return fEmailCC; }
            set { SetPropertyValue(nameof(EmailCC), ref fEmailCC, value); }
        }

        private string fEmailSubject;
        [Size(SizeAttribute.Unlimited)]
        public string EmailSubject
        {
            get { return fEmailSubject; }
            set { SetPropertyValue(nameof(EmailSubject), ref fEmailSubject, value); }
        }

        private string fEmailBody;
        [Size(SizeAttribute.Unlimited)]
        public string EmailBody
        {
            get { return fEmailBody; }
            set { SetPropertyValue(nameof(EmailBody), ref fEmailBody, value); }
        }

        private string fSourceFilePath;
        [Size(SizeAttribute.Unlimited)]
        public string SourceFilePath
        {
            get { return fSourceFilePath; }
            set { SetPropertyValue(nameof(SourceFilePath), ref fSourceFilePath, value); }
        }

        private string fTargetFilePath;
        [Size(200)]
        public string TargetFilePath
        {
            get { return fTargetFilePath; }
            set { SetPropertyValue(nameof(TargetFilePath), ref fTargetFilePath, value); }
        }

        private ReportData fReportData;
        public ReportData ReportData
        {
            get { return fReportData; }
            set { SetPropertyValue(nameof(ReportData), ref fReportData, value); }
        }

        private ReportExportFileFormatEnum? fExportFileFormat;
        public ReportExportFileFormatEnum? ExportFileFormat
        {
            get { return fExportFileFormat; }
            set { SetPropertyValue(nameof(ExportFileFormat), ref fExportFileFormat, value); }
        }

        protected override void OnSaving()
        {
            base.OnSaving();

            if (DoSoReportSchedule != null && Status == MessageStatusEnum.Active)
            {
                var sms2Cancel = DoSoReportSchedule.DoSoEmailsCollection.Where(x => x.ExpiredOn == null && x != this && x.Status == MessageStatusEnum.Active && x.EmailTo == EmailTo && x.EmailSubject == EmailSubject && x.ObjectKey == ObjectKey);
                while (sms2Cancel.Any())
                    sms2Cancel.FirstOrDefault()?.CancelMessage("Created New Message", MessageStatusEnum.CancelledByNewMessage);
            }
        }
    }
}
