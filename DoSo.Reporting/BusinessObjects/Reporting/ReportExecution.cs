using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using NewBaseModule.BisinessObjects;
using DoSo.Reporting.BusinessObjects.Reporting;
using System.ComponentModel;
using DevExpress.XtraSpreadsheet;

namespace DoSo.Reporting.BusinessObjects
{
    [DefaultClassOptions]
    [NavigationItem("Reports")]
    public class ReportExecution : NewXPLiteObjectEx
    {
        public ReportExecution(Session session) : base(session) { }

        private DoSoReport fDoSoReport;
        [ImmediatePostData]
        public DoSoReport DoSoReport
        {
            get { return fDoSoReport; }
            set { SetPropertyValue(nameof(DoSoReport), ref fDoSoReport, value); }
        }


        [Browsable(false)]
        public SpreadsheetControl SpreadsheetControl;
        //private List<ReportDefinition> FilteredReportDefinitions
        //{
        //    get
        //    {
        //        var currentUser = Session.GetObjectByKey<SecuritySystemUser>((SecuritySystem.CurrentUser as SecuritySystemUser)?.Oid) as SecuritySystemUser;
        //        var allActiveReports = Session.Query<ReportDefinition>().Where(x => x.ExpiredOn == null);

        //        if (currentUser == null || currentUser.Roles.Any(x => x.IsAdministrative))
        //            return allActiveReports.ToList();
        //        else
        //        {
        //            //List<ReportDefinition> list = allActiveReports.Where(x => x.ReportDefinitionGroup == null).ToList();

        //            //var reports = allActiveReports.Where(x =>
        //            //    x.ReportDefinitionGroup.With(w => w.UsersCollection).With(w => w.Any(u => u.User == currentUser && u.ExpiredOn == null)) ||
        //            //    x.ReportDefinitionGroup.With(w => w.RolesCollection).With(w => w.Any(a =>
        //            //                currentUser.Roles.Any(ca => ca.With(o =>
        //            //                    o.Oid) == a.With(o => o.Role).With(o =>
        //            //                        o.Oid)) && a.ExpiredOn == null)));

        //            var list = allActiveReports.Where(x => x.ReportDefinitionGroupsCollection.All(w => w.ExpiredOn != null)).ToList();

        //            var groups = Session.Query<ReportDefinitionGroup>().Where(x => x.ExpiredOn == null);
        //            foreach (var item in groups)
        //            {
        //                if (item.UsersCollection.Any(x => x.User.Oid == currentUser.Oid && x.ExpiredOn == null) || item.RolesCollection.Any(x => currentUser.Roles.Any(ca => ca.Oid == x.Role.Oid) && x.ExpiredOn == null))
        //                {
        //                    list.AddRange(item.DefinitionsCollection);
        //                }
        //            }
        //            //&& (x.UsersCollection.Any(u=>u.User.Oid == currentUser.Oid) || x.RolesCollection.Any(a => currentUser.Roles.Any(ca => ca.Oid == a.Role.Oid))));
        //            //var reports = allActiveReports.Where(x =>
        //            //    x.ReportDefinitionGroupsCollection.Any(a => a.UsersCollection.Any(u => u.User == currentUser && u.ExpiredOn == null)) ||
        //            //x.ReportDefinitionGroupsCollection.Any(r => r.RolesCollection.Any(a => currentUser.Roles.Any(ca => ca.Oid == a.Role.Oid) && a.ExpiredOn == null)));

        //            //list.AddRange(reports);

        //            return list;
        //        }
        //    }
        //}

        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            if (propertyName == nameof(DoSoReport) && DoSoReport != null)
            {
                //var parameters = ReportDefinition.QueryParametersCollection;
                //foreach (var queryParameter in parameters)
                //{
                //    if (queryParameter.DataType == DataTypeEnnum.Enum)
                //    {
                //        var enumType = Type.GetType(queryParameter.EnumType.Name);
                //        if (enumType != null)
                //        {
                //            var enumValue = Enum.Parse(enumType, queryParameter.DefaultValue);
                //            queryParameter.ParameterValue = enumValue;
                //        }
                //    }
                //    else
                //        queryParameter.ParameterValue = queryParameter.DefaultValue;
                //}
            }

            base.OnChanged(propertyName, oldValue, newValue);
        }

        private string fExecutedQueries;
        [Size(SizeAttribute.Unlimited)]
        public string ExecutedQueries
        {
            get { return fExecutedQueries; }
            set { SetPropertyValue(nameof(ExecutedQueries), ref fExecutedQueries, value); }
        }

        protected override void OnSaving()
        {
            base.OnSaving();
        }
    }

    public class DoSoQueryParameter
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
