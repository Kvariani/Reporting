using DevExpress.Xpo;
using NewBaseModule.BisinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoSo.Reporting.BusinessObjects.Base
{
    public class DoSoExceptionLog : NewXPLiteObjectEx
    {
        public DoSoExceptionLog(Session session) : base(session) { }
        
        private string fMessage;
        [Size(SizeAttribute.Unlimited)]
        public string Message
        {
            get { return fMessage; }
            set { SetPropertyValue(nameof(Message), ref fMessage, value); }
        }

        private string fStackTrace;
        [Size(SizeAttribute.Unlimited)]
        public string StackTrace
        {
            get { return fStackTrace; }
            set { SetPropertyValue(nameof(StackTrace), ref fStackTrace, value); }
        }

        private int fLevel;
        public int Level
        {
            get { return fLevel; }
            set { SetPropertyValue(nameof(Level), ref fLevel, value); }
        }
    }
}
