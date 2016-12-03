using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DoSo.Reporting.BusinessObjects.Base;
using System.ComponentModel;
using System.Linq;

namespace DoSo.Reporting.BusinessObjects.SMS
{
    //[DefaultClassOptions]
    //[NavigationItem("Reports")]
    //[ImageName("BO_Contact")]
    [DefaultProperty("SmsText")]
    [CreatableItem(false)]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (http://documentation.devexpress.com/#Xaf/CustomDocument2701).
    public class DoSoSms : DoSoMessageBase
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (http://documentation.devexpress.com/#Xaf/CustomDocument3146).
        public DoSoSms(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (http://documentation.devexpress.com/#Xaf/CustomDocument2834).
        }

        private DoSoSmsSchedule fDoSoSmsSchedule;
        [Association("DoSoSmsSchedule-DoSoSms")]
        public DoSoSmsSchedule DoSoSmsSchedule
        {
            get { return fDoSoSmsSchedule; }
            set { SetPropertyValue("DoSoSmsSchedule", ref fDoSoSmsSchedule, value); }
        }

        private string fSmsTo;
        //[ElementTypeProperty("TargetObjectType")]
        public string SmsTo
        {
            get { return fSmsTo; }
            set { SetPropertyValue("SmsTo", ref fSmsTo, value); }
        }

        private string fSmsText;
        [Size(SizeAttribute.Unlimited)]
        //[ElementTypeProperty("TargetObjectType")]
        public string SmsText
        {
            get { return fSmsText; }
            set { SetPropertyValue("SmsText", ref fSmsText, value); }
        }

        protected override void OnSaving()
        {
            base.OnSaving();

            if (DoSoSmsSchedule != null)
            {
                var sms2Cancel = DoSoSmsSchedule.SmsCollection.Where(x => x.ExpiredOn == null && x != this && x.Status == MessageStatusEnum.Active && x.SmsTo == SmsTo && x.SmsText == SmsText);
                while (sms2Cancel.Any())
                    sms2Cancel.FirstOrDefault().CancelMessage("Created New Message", MessageStatusEnum.CancelledByNewMessage);
            }
        }
    }
}
