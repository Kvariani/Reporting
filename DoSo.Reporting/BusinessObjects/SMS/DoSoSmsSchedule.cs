using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DoSo.Reporting.BusinessObjects.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DoSo.Reporting.BusinessObjects.SMS
{
    [DefaultClassOptions]
    //[NavigationItem("Reports")]
    public class DoSoSmsSchedule : DoSoScheduleBase
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (http://documentation.devexpress.com/#Xaf/CustomDocument3146).
        public DoSoSmsSchedule(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (http://documentation.devexpress.com/#Xaf/CustomDocument2834).
        }

        private bool fAllowUnicodeText;
        public bool AllowUnicodeText
        {
            get { return fAllowUnicodeText; }
            set { SetPropertyValue("AllowUnicodeText", ref fAllowUnicodeText, value); }
        }

        [Association("DoSoSmsSchedule-DoSoSms")]
        public XPCollection<DoSoSms> SmsCollection => GetCollection<DoSoSms>("SmsCollection");

        private string fSmsTo;
        [ElementTypeProperty("TargetObjectType")]
        [ModelDefault("PropertyEditorType", "DoSo.Reporting.Controllers.PopupExpressionPropertyEditorEx")]
        public string SmsTo
        {
            get { return fSmsTo; }
            set { SetPropertyValue("SmsTo", ref fSmsTo, value); }
        }

        private string fSmsText;
        [Size(SizeAttribute.Unlimited)]
        [ElementTypeProperty("TargetObjectType")]
        [ModelDefault("PropertyEditorType", "DoSo.Reporting.Controllers.PopupExpressionPropertyEditorEx")]
        public string SmsText
        {
            get { return fSmsText; }
            set { SetPropertyValue("SmsText", ref fSmsText, value); }
        }


        public override List<DoSoMessageBase> GenerateMessages(Session session, bool prevewOnly = false)
        {
            base.GenerateMessages(session);

            var itemsList = GetObjectsFromDataSource();
            var properties = session.GetProperties(itemsList.FirstOrDefault().ClassInfo);
            var smses = new List<DoSoMessageBase>();

            foreach (var item in itemsList)
            {
                var sms = new DoSoSms(session)
                {
                    SmsText = new ExpressionEvaluator(properties, SmsText).Evaluate(item)?.ToString(),
                    SmsTo = new ExpressionEvaluator(properties, SmsTo).Evaluate(item)?.ToString(),
                    SendingDate = Convert.ToDateTime(new ExpressionEvaluator(properties, SendingDateExpression).Evaluate(item)),
                    ObjectKey = new ExpressionEvaluator(properties, ObjectKeyExpression).Evaluate(item)?.ToString(),
                    ObjectTypeName = new ExpressionEvaluator(properties, ObjectTypeExpression).Evaluate(item)?.ToString(),
                    DoSoSmsSchedule = session.GetObjectByKey<DoSoSmsSchedule>(ID)
                };

                if (SkipExecutionDate != null)
                {
                    var sameSms = SmsCollection.Where(x => x.ExpiredOn == null && x.Status == DoSoMessageBase.MessageStatusEnum.Skipped && sms.SendingDate.AddDays(SkipExecutionDate ?? 0) > x.SentDate && x.SmsTo == sms.SmsTo && x.SmsText == sms.SmsText);
                    if (sameSms.Any())
                        sms.CancelMessage("Skipped", DoSoMessageBase.MessageStatusEnum.Skipped);
                }

                smses.Add(sms);
            }

            return smses;
        }


    }
}
