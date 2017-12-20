using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Validation.AllContextsView;
using DevExpress.ExpressApp.Win.SystemModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoSo.Reporting.Controllers
{
    public class DisableUnusedControllers : ViewController
    {
        protected override void OnActivated()
        {
            base.OnActivated();


            Frame.GetController<RecordsNavigationController>()?.Active?.SetItemValue("", false);
            Frame.GetController<ModificationsController>()?.SaveAndNewAction.Active.SetItemValue("", false);
            Frame.GetController<ResetViewSettingsController>()?.Active.SetItemValue("", false);
            Frame.GetController<ShowAllContextsController>().ValidateAction.Active.SetItemValue("", false);
            Frame.GetController<LinkUnlinkController>().Active.SetItemValue("", false);
            Frame.GetController<CloseWindowController>()?.Active?.SetItemValue("", false);
            Frame.GetController<OpenObjectController>()?.Active?.SetItemValue("", false);
        }
    }
}
