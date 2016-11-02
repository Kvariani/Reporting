using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoSo.Reporting.Controllers
{
    public class DIsableUnusedControllers : ViewController
    {
        protected override void OnActivated()
        {
            base.OnActivated();


            Frame.GetController<RecordsNavigationController>()?.Active?.SetItemValue("", false);
        }
    }
}
