using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using FastExcelExportingDemoCs;
using Reporting.Reporting.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using NewBaseModule.BisinessObjects;

namespace DoSo.Reporting.BusinessObjects
{
    [DefaultClassOptions]
    [NavigationItem("Reports")]
    public class ReportExecution : NewXPLiteObjectEx
    {
        public ReportExecution(Session session) : base(session) { }

        private ReportDefinition fReportDefinition;
        [Association("ReportDefinition-ReportExecution")]
        [DataSourceProperty(nameof(FilteredReportDefinitions))]
        [ImmediatePostData]
        public ReportDefinition ReportDefinition
        {
            get { return fReportDefinition; }
            set { SetPropertyValue(nameof(ReportDefinition), ref fReportDefinition, value); }
        }

        private List<ReportDefinition> FilteredReportDefinitions
        {
            get
            {
                var currentUser = Session.GetObjectByKey<SecuritySystemUser>((SecuritySystem.CurrentUser as SecuritySystemUser)?.Oid) as SecuritySystemUser;
                var allActiveReports = Session.Query<ReportDefinition>().Where(x => x.ExpiredOn == null);

                if (currentUser == null || currentUser.Roles.Any(x => x.IsAdministrative))
                    return allActiveReports.ToList();
                else
                {
                    //List<ReportDefinition> list = allActiveReports.Where(x => x.ReportDefinitionGroup == null).ToList();

                    //var reports = allActiveReports.Where(x =>
                    //    x.ReportDefinitionGroup.With(w => w.UsersCollection).With(w => w.Any(u => u.User == currentUser && u.ExpiredOn == null)) ||
                    //    x.ReportDefinitionGroup.With(w => w.RolesCollection).With(w => w.Any(a =>
                    //                currentUser.Roles.Any(ca => ca.With(o =>
                    //                    o.Oid) == a.With(o => o.Role).With(o =>
                    //                        o.Oid)) && a.ExpiredOn == null)));

                    var list = allActiveReports.Where(x => x.ReportDefinitionGroupsCollection.All(w => w.ExpiredOn != null)).ToList();

                    var groups = Session.Query<ReportDefinitionGroup>().Where(x => x.ExpiredOn == null);
                    foreach (var item in groups)
                    {
                        if (item.UsersCollection.Any(x => x.User.Oid == currentUser.Oid && x.ExpiredOn == null) || item.RolesCollection.Any(x => currentUser.Roles.Any(ca => ca.Oid == x.Role.Oid) && x.ExpiredOn == null))
                        {
                            list.AddRange(item.DefinitionsCollection);
                        }
                    }
                    //&& (x.UsersCollection.Any(u=>u.User.Oid == currentUser.Oid) || x.RolesCollection.Any(a => currentUser.Roles.Any(ca => ca.Oid == a.Role.Oid))));
                    //var reports = allActiveReports.Where(x =>
                    //    x.ReportDefinitionGroupsCollection.Any(a => a.UsersCollection.Any(u => u.User == currentUser && u.ExpiredOn == null)) ||
                    //x.ReportDefinitionGroupsCollection.Any(r => r.RolesCollection.Any(a => currentUser.Roles.Any(ca => ca.Oid == a.Role.Oid) && a.ExpiredOn == null)));

                    //list.AddRange(reports);

                    return list;
                }
            }
        }

        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            if (propertyName == nameof(ReportDefinition) && ReportDefinition != null)
            {
                var parameters = ReportDefinition.QueryParametersCollection;
                foreach (var queryParameter in parameters)
                {
                    if (queryParameter.DataType == DataTypeEnnum.Enum)
                    {
                        var enumType = Type.GetType(queryParameter.EnumType.Name);
                        if (enumType != null)
                        {
                            var enumValue = Enum.Parse(enumType, queryParameter.DefaultValue);
                            queryParameter.ParameterValue = enumValue;
                        }
                    }
                    else
                        queryParameter.ParameterValue = queryParameter.DefaultValue;
                }
            }

            base.OnChanged(propertyName, oldValue, newValue);
        }

        private string fExecutedQuery;
        [Size(SizeAttribute.Unlimited)]
        public string ExecutedQuery
        {
            get { return fExecutedQuery; }
            set { SetPropertyValue(nameof(ExecutedQuery), ref fExecutedQuery, value); }
        }

        protected override void OnSaving()
        {
            base.OnSaving();

            ExecutedQuery = FastExportingMethod.ExecutedQueries;
        }
    }
}
