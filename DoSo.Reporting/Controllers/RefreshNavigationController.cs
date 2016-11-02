using System;
using DevExpress.ExpressApp;
using DoSo.Reporting.BusinessObjects;

namespace Common.Win.General.DashBoard.Controllers
{
    public partial class RefreshNavigationController : ViewController
    {
        bool refreshdashboards;

        protected RefreshNavigationController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetObjectType = typeof(DoSoDashboard);
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            ObjectSpace.ObjectChanged += ObjectSpace_ObjectChanged;
            ObjectSpace.Committed += ObjectSpace_Committed;
        }

        protected override void OnDeactivated()
        {
            ObjectSpace.ObjectChanged -= ObjectSpace_ObjectChanged;
            ObjectSpace.Committed -= ObjectSpace_Committed;
            base.OnDeactivated();
        }

        void ObjectSpace_ObjectChanged(object sender, ObjectChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
                refreshdashboards = true;
        }

        void ObjectSpace_Committed(object sender, EventArgs e)
        {
            if (refreshdashboards)
                Frame.Application.MainWindow.GetController<DashboardNavigationController>().RecreateNavigationItems();
        }
    }
}
