using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System;
using NewBaseModule.BisinessObjects;

namespace DoSo.Reporting.BusinessObjects.Base
{
    //[DefaultClassOptions]
    //[NavigationItem("Reports")]
    [DefaultClassOptions]
    public class DoSoMessageBase : NewXPLiteObjectEx
    {
        public DoSoMessageBase(Session session) : base(session) { }

        public override void AfterConstruction()
        {
            base.AfterConstruction();

            Status = MessageStatusEnum.Active;
        }

        //private bool fIsSent;
        //public bool IsSent
        //{
        //    get { return fIsSent; }
        //    set { SetPropertyValue(nameof(IsSent), ref fIsSent, value); }
        //}

        //private bool fIsCanceled;
        //public bool IsCanceled
        //{
        //    get { return fIsCanceled; }
        //    set { SetPropertyValue(nameof(IsCanceled), ref fIsCanceled, value); }
        //}



        private DateTime fSendingDate;
        public DateTime SendingDate
        {
            get { return fSendingDate; }
            set { SetPropertyValue(nameof(SendingDate), ref fSendingDate, value); }
        }

        private DateTime fSentDate;
        public DateTime SentDate
        {
            get { return fSentDate; }
            set { SetPropertyValue(nameof(SentDate), ref fSentDate, value); }
        }

        private string fObjectTypeName;
        public string ObjectTypeName
        {
            get { return fObjectTypeName; }
            set { SetPropertyValue(nameof(ObjectTypeName), ref fObjectTypeName, value); }
        }

        private string fObjectKey;
        public string ObjectKey
        {
            get { return fObjectKey; }
            set { SetPropertyValue(nameof(ObjectKey), ref fObjectKey, value); }
        }

        private string fStatusComment;
        [ModelDefault("AllowEdit", "False")]
        [Size(SizeAttribute.Unlimited)]
        public string StatusComment
        {
            get { return fStatusComment; }
            set { SetPropertyValue(nameof(StatusComment), ref fStatusComment, value); }
        }

        private MessageStatusEnum fStatus;
        [ModelDefault("AllowEdit", "False")]
        public MessageStatusEnum Status
        {
            get { return fStatus; }
            set { SetPropertyValue(nameof(Status), ref fStatus, value); }
        }

        public void CancelMessage(string comment, MessageStatusEnum status)
        {
            Status = status;
            //IsCanceled = true;
            StatusComment = comment;
        }

        public enum MessageStatusEnum
        {
            Active = 0,
            Sent = 1,
            CancelledByUser = 2,
            CancelledByService = 3,
            CancelledByNewMessage = 4,
            Skipped = 5
        }
    }
}
