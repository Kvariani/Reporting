using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using NewBaseModule.BisinessObjects;

namespace DoSo.Reporting.BusinessObjects.Reporting
{
    [DefaultClassOptions]
    //[DoSoManyToMany]
    public class User2Report : NewXPLiteObjectEx
    {
        public User2Report(Session session) : base(session) { }

        private ReportDefinitionGroup fReportDefinitionGroup;
        [Association("ReportDefinitionGroup-User2Report")]
        public ReportDefinitionGroup ReportDefinitionGroup
        {
            get { return fReportDefinitionGroup; }
            set { SetPropertyValue(nameof(ReportDefinitionGroup), ref fReportDefinitionGroup, value); }
        }

        private SecuritySystemUser fUser;
        public SecuritySystemUser User
        {
            get { return fUser; }
            set { SetPropertyValue(nameof(User), ref fUser, value); }
        }
    }
}
