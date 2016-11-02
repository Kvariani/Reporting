using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using NewBaseModule.BisinessObjects;

namespace DoSo.Reporting.BusinessObjects.Reporting
{
    [DefaultClassOptions]
    public class Role2Report : NewXPLiteObjectEx
    {
        public Role2Report(Session session) : base(session) { }

        private ReportDefinitionGroup fReportDefinitionGroup;
        [Association("ReportDefinitionGroup-Role2Report")]
        public ReportDefinitionGroup ReportDefinitionGroup
        {
            get { return fReportDefinitionGroup; }
            set { SetPropertyValue(nameof(ReportDefinitionGroup), ref fReportDefinitionGroup, value); }
        }

        private SecuritySystemRole fRole;
        public SecuritySystemRole Role
        {
            get { return fRole; }
            set { SetPropertyValue(nameof(Role), ref fRole, value); }
        }
    }
}
