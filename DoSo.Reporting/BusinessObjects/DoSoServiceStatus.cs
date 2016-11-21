using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoSo.Reporting.BusinessObjects
{
    public class DoSoServiceStatus : XPLiteObject
    {

        public DoSoServiceStatus(Session session) : base(session) { }

        private string fServiceName;
        [Key(false)]
        public string ServiceName
        {
            get { return fServiceName; }
            set { SetPropertyValue(nameof(ServiceName), ref fServiceName, value); }
        }

        private DateTime fLastActiveTime;
        public DateTime LastActiveTime
        {
            get { return fLastActiveTime; }
            set { SetPropertyValue(nameof(LastActiveTime), ref fLastActiveTime, value); }
        }

        private DateTime fLastExceptionTime;
        public DateTime LastExceptionTime
        {
            get { return fLastExceptionTime; }
            set { SetPropertyValue(nameof(LastExceptionTime), ref fLastExceptionTime, value); }
        }
    }
}
