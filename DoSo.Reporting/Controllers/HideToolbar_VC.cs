using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.SystemModule;
using DoSo.Reporting.BusinessObjects;

namespace DoSo.Reporting.Controllers
{

    public partial class HideToolbar_VC : ViewController
    {
        public HideToolbar_VC()
        {
            InitializeComponent();
            RegisterActions(components);
            TypeOfView = typeof(ListView);
            TargetViewNesting = Nesting.Nested;
            TargetObjectType = typeof(QueryParameter);
            
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();

            var ctrl = Frame.GetController<ToolbarVisibilityController>();
            ctrl.SetToolbarVisibility(false);
            
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            var masterobject = ((View as ListView).CollectionSource as PropertyCollectionSource).MasterObject;

            //if (masterobject is ReportExecution)
            //{
            //    var openAction = Frame.GetController<ListViewProcessCurrentObjectController>().ProcessCurrentObjectAction;

            //    var myList = openAction.Enabled;
            //    myList["enableaction"] = false;
            //}

        }
    }
}
