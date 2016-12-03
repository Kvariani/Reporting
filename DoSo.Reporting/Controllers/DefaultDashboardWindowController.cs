using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoSo.Reporting.Controllers
{
    public class DefaultDashboardWindowController : WindowController
    {
        public DefaultDashboardWindowController()
        {
            TargetWindowType = WindowType.Main;
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            Frame.GetController<ShowNavigationItemController>().CustomShowNavigationItem += DefaultDashboardController_CustomShowNavigationItem;
        }

        void DefaultDashboardController_CustomShowNavigationItem(object sender, CustomShowNavigationItemEventArgs e)
        {
            if (e.ActionArguments.SelectedChoiceActionItem.Id == $"{nameof(MainDashboard)}_ListView")
            {
                IObjectSpace objectSpace = Application.CreateObjectSpace();
                var objectToShow = new MainDashboard();
                e.ActionArguments.ShowViewParameters.CreatedView = Application.CreateDetailView(objectSpace, objectToShow, true);
                e.ActionArguments.ShowViewParameters.TargetWindow = TargetWindow.Current;
                e.ActionArguments.ShowViewParameters.CreatedView.QueryCanClose += CreatedView_QueryCanClose;
                e.Handled = true;
            }
        }

        void CreatedView_QueryCanClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //var currentObject = (sender as DetailView).CurrentObject;
            //XPObjectSpace.FindObjectSpaceByObject(currentObject).RemoveFromModifiedObjects(currentObject);
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            Frame.GetController<ShowNavigationItemController>().CustomShowNavigationItem -= DefaultDashboardController_CustomShowNavigationItem;
        }
    }

    [DefaultClassOptions]
    [DomainComponent]
    public class MainDashboard
    {

    }
}
