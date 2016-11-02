using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using NewBaseModule.BisinessObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace DoSo.Reporting.BusinessObjects.Base
{
    [DefaultClassOptions]
    [NavigationItem("Reports")]
    public class MessageGeneratorRule : NewXPLiteObjectEx
    {
        public MessageGeneratorRule(Session session) : base(session) { }

        private bool fIsActive;
        public bool IsActive
        {
            get { return fIsActive; }
            set { SetPropertyValue(nameof(IsActive), ref fIsActive, value); }
        }

        private bool fExpireNotSentMessages;
        public bool ExpireNotSentMessages
        {
            get { return fExpireNotSentMessages; }
            set { SetPropertyValue(nameof(ExpireNotSentMessages), ref fExpireNotSentMessages, value); }
        }

        public Type BusinessObject
        {
            get
            {
                if (string.IsNullOrEmpty(BusinessObjectFullName))
                    return null;
                var BusinessObjectInfo = XafTypesInfo.Instance.FindTypeInfo(BusinessObjectFullName);
                return BusinessObjectInfo == null ? null : XafTypesInfo.Instance.FindTypeInfo(BusinessObjectFullName).Type;
            }
            set { BusinessObjectFullName = value.FullName; }
        }

        private string fBusinessObjectFullName;
        [Browsable(false)]
        public string BusinessObjectFullName
        {
            get { return fBusinessObjectFullName; }
            set { SetPropertyValue(nameof(BusinessObjectFullName), ref fBusinessObjectFullName, value); }
        }

        private Type TargetObjectType => BusinessObject;

        public Type BusinessObject4MessageTo
        {
            get
            {
                if (string.IsNullOrEmpty(MessageToBusinessObjectFullName))
                    return null;
                var BusinessObjectInfo = XafTypesInfo.Instance.FindTypeInfo(MessageToBusinessObjectFullName);
                return BusinessObjectInfo == null ? null : XafTypesInfo.Instance.FindTypeInfo(MessageToBusinessObjectFullName).Type;
            }
            set { MessageToBusinessObjectFullName = value.FullName; }
        }

        private string fMessageToBusinessObjectFullName;
        [Browsable(false)]
        public string MessageToBusinessObjectFullName
        {
            get { return fMessageToBusinessObjectFullName; }
            set { SetPropertyValue(nameof(MessageToBusinessObjectFullName), ref fMessageToBusinessObjectFullName, value); }
        }

        private Type TargetObjectType4MessageTo => BusinessObject4MessageTo;

        private string fSendingCriteria;
        [CriteriaOptions(nameof(TargetObjectType))]
        [Size(SizeAttribute.Unlimited)]
        public string SendingCriteria
        {
            get { return fSendingCriteria; }
            set { SetPropertyValue(nameof(SendingCriteria), ref fSendingCriteria, value); }
        }

        private string fCancellationCriteria;
        [CriteriaOptions(nameof(TargetObjectType))]
        [Size(SizeAttribute.Unlimited)]
        public string CancellationCriteria
        {
            get { return fCancellationCriteria; }
            set { SetPropertyValue(nameof(CancellationCriteria), ref fCancellationCriteria, value); }
        }

        private string fCustomMessage2ObjectCriteria;
        [CriteriaOptions(nameof(TargetObjectType4MessageTo))]
        [Size(SizeAttribute.Unlimited)]
        public string CustomMessage2ObjectCriteria
        {
            get { return fCustomMessage2ObjectCriteria; }
            set { SetPropertyValue(nameof(CustomMessage2ObjectCriteria), ref fCustomMessage2ObjectCriteria, value); }
        }

        private string fCancellationComment;
        [Size(SizeAttribute.Unlimited)]
        [ElementTypeProperty(nameof(TargetObjectType))]
        public string CancellationComment
        {
            get { return fCancellationComment; }
            set { SetPropertyValue(nameof(CancellationComment), ref fCancellationComment, value); }
        }

        private string fMessageTo;
        [Size(SizeAttribute.Unlimited)]
        [RuleRequiredField]
        [ElementTypeProperty(nameof(TargetObjectType))]
        public string MessageTo
        {
            get { return fMessageTo; }
            set { SetPropertyValue(nameof(MessageTo), ref fMessageTo, value); }
        }

        private string fCustomMessageTo;
        [Size(SizeAttribute.Unlimited)]
        [ElementTypeProperty(nameof(TargetObjectType4MessageTo))]
        public string CustomMessageTo
        {
            get { return fCustomMessageTo; }
            set { SetPropertyValue(nameof(CustomMessageTo), ref fCustomMessageTo, value); }
        }

        private string fMessageText;
        [RuleRequiredField]
        [Size(SizeAttribute.Unlimited)]
        [ElementTypeProperty(nameof(TargetObjectType))]
        public string MessageText
        {
            get { return fMessageText; }
            set { SetPropertyValue(nameof(MessageText), ref fMessageText, value); }
        }

        private string fSendingTime;
        [Size(SizeAttribute.Unlimited)]
        [ElementTypeProperty(nameof(TargetObjectType))]
        public string SendingTime
        {
            get { return fSendingTime; }
            set { SetPropertyValue(nameof(SendingTime), ref fSendingTime, value); }
        }

        private ReportData fReportData;
        [DataSourceProperty(nameof(FilteredReports))]
        public ReportData ReportData
        {
            get { return fReportData; }
            set { SetPropertyValue(nameof(ReportData), ref fReportData, value); }
        }

        private List<ReportData> FilteredReports
        {
            get { return Session.Query<ReportData>().Where(rd => rd.DataTypeName == BusinessObject.FullName).ToList(); }
        }

        private ReportExportFileFormatEnum fExportFileFormat;
        public ReportExportFileFormatEnum ExportFileFormat
        {
            get { return fExportFileFormat; }
            set { SetPropertyValue(nameof(ExportFileFormat), ref fExportFileFormat, value); }
        }
    }
}
