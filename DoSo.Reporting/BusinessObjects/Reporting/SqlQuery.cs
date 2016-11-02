using System;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Reporting.Reporting.BusinessObjects;
using System.Linq;
using DevExpress.ExpressApp.Model;
using DoSo.Reporting.BusinessObjects.Reporting;
using NewBaseModule.BisinessObjects;

namespace DoSo.Reporting.BusinessObjects
{
    [DefaultClassOptions]
    [NavigationItem("Reports")]
    [Serializable]
    public class SqlQuery : NewXPLiteObjectEx
    {
        public SqlQuery(Session session) : base(session) { }

        private string fName;
        [RuleRequiredField]
        public string Name
        {
            get { return fName; }
            set { SetPropertyValue(nameof(Name), ref fName, value); }
        }

        private string fTittle;
        [Size(SizeAttribute.Unlimited)]
        //[RuleRequiredField]
        public string Tittle
        {
            get { return fTittle; }
            set { SetPropertyValue(nameof(Tittle), ref fTittle, value); }
        }

        private string fQuery;
        [Size(SizeAttribute.Unlimited)]
        [RuleRequiredField]
        public string Query
        {
            get { return fQuery; }
            set { SetPropertyValue(nameof(Query), ref fQuery, value); }
        }

        //private int fStartColumn;
        //public int StartColumn
        //{
        //    get { return fStartColumn; }
        //    set { SetPropertyValue(nameof(StartColumn), ref fStartColumn, value); }
        //}

        //private int fStartRow;
        //public int StartRow
        //{
        //    get { return fStartRow; }
        //    set { SetPropertyValue(nameof(StartRow), ref fStartRow, value); }
        //}

        private string fRange;
        [ModelDefault("EditMask", @"[A-Z]{1}[0-9]{1}")]
        [ModelDefault("EditMaskType", "RegEx")]
        public string Range
        {
            get { return fRange; }
            set { SetPropertyValue(nameof(Range), ref fRange, value); }
        }

        private int fSheetIndex;
        public int SheetIndex
        {
            get { return fSheetIndex; }
            set { SetPropertyValue(nameof(SheetIndex), ref fSheetIndex, value); }
        }

        private ReportDefinition fReportDefinition;
        [Association("ReportDefinition-SqlQuery")]
        public ReportDefinition ReportDefinition
        {
            get { return fReportDefinition; }
            set { SetPropertyValue(nameof(ReportDefinition), ref fReportDefinition, value); }
        }

        //[Association]
        //public XPCollection<CustomReportHeader> ReportHEadersCollection => GetCollection<CustomReportHeader>(nameof(ReportHEadersCollection));


        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);

            if (propertyName == nameof(ReportDefinition) && ReportDefinition != null)
            {
                var existingQueries = ReportDefinition.SqlQueryCollection.Where(x => x.ExpiredOn == null).ToList();
                if (existingQueries.Any())
                    SheetIndex = existingQueries.Max(x => x.SheetIndex) + 1;
                else
                    SheetIndex = 1;
            }
        }

        protected override void OnSaving()
        {
            base.OnSaving();

            var splitQuery = Query.Replace(")", " ").Replace("\r\n", " ").Replace("\t", " ").Replace(";", "").Split(' ').Where(s => s.StartsWith("@")).Distinct();
            if (ReportDefinition != null)
            {
                if (ReportDefinition.SqlQueryCollection.Any(x => x.SheetIndex == SheetIndex && x != this))
                    throw new InvalidOperationException("მითითებული ინდექსით უკვე არის შექმნილი ჩანაწერი");


                throw new InvalidOperationException("Parameter query");
                //var parametersToCreate = splitQuery.Where(s => !s.In(from rep in ReportDefinition.QueryParametersCollection select rep.ParameterName));

                //foreach (var newParameterName in parametersToCreate)
                //{
                //    if (ReportDefinition.QueryParametersCollection.Any(x => x.ParameterName.ToLower() == newParameterName.ToLower()))
                //        continue;

                //    new QueryParameter(Session)
                //    {
                //        ParameterName = newParameterName,
                //        ReportDefinition = this.ReportDefinition
                //    };
                //}
            }
        }
    }
}

