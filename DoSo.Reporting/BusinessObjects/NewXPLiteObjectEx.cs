using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewBaseModule.BisinessObjects
{
    [NonPersistent]
    public abstract class NewXPLiteObjectEx : XPLiteObject
    {
        protected NewXPLiteObjectEx(Session session) : base(session) { }


        private int fID;
        [Key(true)]
        public int ID
        {
            get { return fID; }
            set { SetPropertyValue(nameof(ID), ref fID, value); }
        }

        private DateTime? fExpiredOn;
        public DateTime? ExpiredOn
        {
            get { return fExpiredOn; }
            set { SetPropertyValue(nameof(ExpiredOn), ref fExpiredOn, value); }
        }

        private DateTime? fCreatedOn;
        public DateTime? CreatedOn
        {
            get { return fCreatedOn; }
            set { SetPropertyValue(nameof(CreatedOn), ref fCreatedOn, value); }
        }

        protected override void OnSaving()
        {
            base.OnSaving();

            CreatedOn = DateTime.Now;
        }

        public virtual void ExpireObject()
        {
            ExpiredOn = DateTime.Now;
        }
    }
}
