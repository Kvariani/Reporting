using DevExpress.ExpressApp.Core;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DoSo.Reporting.BusinessObjects.Base;

namespace DoSo.Reporting.BusinessObjects.Email
{
    [DefaultClassOptions]
    [NavigationItem("Reports")]
    public class DoSoEmailGeneratorRule : MessageGeneratorRule
    {
        public DoSoEmailGeneratorRule(Session session) : base(session) { }

        private string fMessageSubject;
        [Size(SizeAttribute.Unlimited)]
        [ElementTypeProperty("TargetObjectType")]
        public string MessageSubject
        {
            get { return fMessageSubject; }
            set { SetPropertyValue(nameof(MessageSubject), ref fMessageSubject, value); }
        }

        private string fMessageCC;
        [Size(SizeAttribute.Unlimited)]
        //[ElementTypeProperty("TargetObjectType")]
        public string MessageCC
        {
            get { return fMessageCC; }
            set { SetPropertyValue(nameof(MessageCC), ref fMessageCC, value); }
        }


        //private XPBusinessObjectMember fAttachedFileProperty;
        //[DataSourceCriteria("BusinessObject.BusinessObjectFullName == '@this.BusinessObjectFullName'")]
        //public XPBusinessObjectMember AttachedFileProperty
        //{
        //    get { return fAttachedFileProperty; }
        //    set { SetPropertyValue(nameof(AttachedFileProperty), ref fAttachedFileProperty, value); }
        //}

        private string fGeneratedFilePath;
        [Size(200)]
        public string GeneratedFilePath
        {
            get { return fGeneratedFilePath; }
            set { SetPropertyValue(nameof(GeneratedFilePath), ref fGeneratedFilePath, value); }
        }
    }
}
