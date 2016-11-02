using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DoSo.Reporting.BusinessObjects.Base;

namespace DoSo.Reporting.BusinessObjects.SMS
{
    [DefaultClassOptions]
    [NavigationItem("Reports")]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (http://documentation.devexpress.com/#Xaf/CustomDocument2701).
    public class DoSoSmsGeneratorRule : MessageGeneratorRule
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (http://documentation.devexpress.com/#Xaf/CustomDocument3146).
        public DoSoSmsGeneratorRule(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (http://documentation.devexpress.com/#Xaf/CustomDocument2834).
        }


    }
}
