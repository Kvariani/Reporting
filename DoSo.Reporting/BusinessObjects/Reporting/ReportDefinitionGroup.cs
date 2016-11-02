using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DoSo.Reporting.BusinessObjects.Reporting;
using NewBaseModule.BisinessObjects;
using Reporting.Reporting.BusinessObjects;

namespace DoSo.Reporting.BusinessObjects
{
    [DefaultClassOptions]
    [NavigationItem("Reports")]
    public class ReportDefinitionGroup : NewXPLiteObjectEx
    {
        public ReportDefinitionGroup(Session session) : base(session) { }

        private string fName;
        public string Name
        {
            get { return fName; }
            set { SetPropertyValue(nameof(Name), ref fName, value); }
        }

        private string fDescription;
        [Size(SizeAttribute.Unlimited)]
        public string Description
        {
            get { return fDescription; }
            set { SetPropertyValue(nameof(Description), ref fDescription, value); }
        }

        [Association("Group-Definition")]
        public XPCollection<ReportDefinition> DefinitionsCollection => GetCollection<ReportDefinition>(nameof(DefinitionsCollection));

        //[Association("ReportDefinitionGroup-ReportDefinition")]
        //public XPCollection<ReportDefinition> ReportDefinitionsCollection => GetCollection<ReportDefinition>(nameof(ReportDefinitionsCollection));

        [Association("ReportDefinitionGroup-Role2Report")]
        public XPCollection<Role2Report> RolesCollection => GetCollection<Role2Report>(nameof(RolesCollection));

        [Association("ReportDefinitionGroup-User2Report")]
        public XPCollection<User2Report> UsersCollection => GetCollection<User2Report>(nameof(UsersCollection));

        private ReportDefinitionGroup fParentID;
        public ReportDefinitionGroup ParentID
        {
            get { return fParentID; }
            set { SetPropertyValue(nameof(ParentID), ref fParentID, value); }
        }
    }
}
