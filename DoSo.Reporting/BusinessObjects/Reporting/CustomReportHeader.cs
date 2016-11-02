using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using NewBaseModule.BisinessObjects;

namespace DoSo.Reporting.BusinessObjects.Reporting
{
    public class CustomReportHeader : NewXPLiteObjectEx
    {
        public CustomReportHeader(Session session) : base(session) { }


        private string fText;
        [Size(SizeAttribute.Unlimited)]
        public string Text
        {
            get { return fText; }
            set { SetPropertyValue(nameof(Text), ref fText, value); }
        }


        private string fRange;
        [ModelDefault("EditMask", @"[A-Z]{1}[0-9]{1}:[A-Z]{1}[0-9]{1}")]
        [ModelDefault("EditMaskType", "RegEx")]
        [RuleRequiredField]
        public string Range
        {
            get { return fRange; }
            set { SetPropertyValue(nameof(Range), ref fRange, value); }
        }

        private SqlQuery fSqlQuery;
        [Association]
        public SqlQuery SqlQuery
        {
            get { return fSqlQuery; }
            set { SetPropertyValue(nameof(SqlQuery), ref fSqlQuery, value); }
        }
    }
}
