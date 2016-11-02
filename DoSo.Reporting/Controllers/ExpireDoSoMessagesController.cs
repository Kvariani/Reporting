using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DoSo.Reporting.BusinessObjects.Base;
using System;
using System.Linq;

namespace DoSo.Reporting.Controllers
{
    public partial class ExpireDoSoMessagesController : ObjectViewController<ListView, DoSoMessageBase>
    {
        public ExpireDoSoMessagesController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        private void simpleAction_ExpireMessages_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            foreach (var item in View.SelectedObjects.OfType<DoSoMessageBase>().Where(x => x.ExpiredOn == null))
                item.ExpiredOn = DateTime.Now;

            ObjectSpace.CommitChanges();
        }
    }
}
