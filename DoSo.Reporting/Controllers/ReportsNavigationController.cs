using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using System.ComponentModel;
using DoSo.Reporting.BusinessObjects;
using DevExpress.Xpo;
using DoSo.Reporting.BusinessObjects.Reporting;

namespace DoSo.Reporting.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppWindowControllertopic.aspx.
    public partial class ReportsNavigationController : WindowController, IModelExtender
    {
        ShowNavigationItemController _showNavigationItemController;

        public ReportsNavigationController()
        {
            TargetWindowType = WindowType.Main;
            InitializeComponent();
            // Target required Windows (via the TargetXXX properties) and create their Actions.
        }


        protected override void OnFrameAssigned()
        {
            UnsubscribeFromEvents();
            base.OnFrameAssigned();
            SubscribeToEvents();
        }

        void UnsubscribeFromEvents()
        {
            if (_showNavigationItemController != null)
            {
                _showNavigationItemController.ItemsInitialized -= ShowNavigationItemControllerItemsInitialized;
                _showNavigationItemController = null;
            }
        }

        public ChoiceActionItem GetGroupFromActions(SingleChoiceAction action, String name) => action.Items.FirstOrDefault(item => item.Caption.Equals(name));

        private void ReportsNavigationController_CustomShowNavigationItem(object sender, CustomShowNavigationItemEventArgs e)
        {
            if (e.ActionArguments.SelectedChoiceActionItem.Data is DoSoReport)
            {
                IObjectSpace objectSpace = Application.CreateObjectSpace();
                var objectToShow = objectSpace.CreateObject<ReportExecution>();

                var param = e.ActionArguments.ShowViewParameters;
                param.CreatedView = Application.CreateDetailView(objectSpace, objectToShow, false);
                param.Context = TemplateContext.PopupWindow;
                param.TargetWindow = TargetWindow.NewModalWindow;
                var co = param.Controllers;
                var dialogControler = new DialogController();
                dialogControler.CancelAction.Executing += CancelAction_Executing;
                param.Controllers.Add(dialogControler);
                dialogControler.AcceptAction.Executing += (s, ee) => AcceptAction_Executing(s, ee, objectToShow);
                param.CreatedView.ControlsCreated += (s, ee) => CreatedView_ControlsCreated(s, ee, objectSpace.GetObject(e.ActionArguments.SelectedChoiceActionItem.Data) as DoSoReport);
                e.Handled = true;
            }
        }

        private void CancelAction_Executing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void AcceptAction_Executing(object sender, CancelEventArgs e, ReportExecution exec)
        {
            var view = (sender as SimpleAction).SelectionContext as DetailView;
            exec.DoSoReport.MaybeFast(exec, view, Application);
            e.Cancel = true;
        }

        private void CreatedView_ControlsCreated(object sender, EventArgs e, DoSoReport report)
        {
            var view = sender as DetailView;
            var currentObject = view.CurrentObject as ReportExecution;
            currentObject.DoSoReport = report;

        }

        void ShowNavigationItemControllerItemsInitialized(object sender, EventArgs e)
        {
            IModelView view = Application.FindModelView(Application.FindListViewId(typeof(DoSoDashboard)));
            var options = ((IModelOptionsDashboards)Application.Model.Options);
            var dashboardOptions = ((IModelOptionsDashboardNavigation)options.Dashboards);
            if (dashboardOptions.DashboardsInGroup)
            {
                var navigationGroup = ((ShowNavigationItemController)sender).ShowNavigationItemAction.Items.FirstOrDefault(x => x.Id == "Reports");
                navigationGroup.Items.Clear();
                //ReloadDashboardActions();
                //var actions = new List<ChoiceActionItem>();

                using (var uow = new UnitOfWork(XpoDefault.DataLayer))
                {
                    var reports = uow.Query<DoSoReport>();
                    foreach (var item in reports)
                    {
                        //var actionItem = new ChoiceActionItem(item.ID.ToString(), item.Name, new ViewShortcut());
                        navigationGroup.Items.Add(new ChoiceActionItem(item.Name, item));
                    }
                }
                //if (DashboardActions.Count > 0)
                //{
                //var dashboardGroup = GetGroupFromActions(((ShowNavigationItemController)sender).ShowNavigationItemAction, dashboardOptions.DashboardGroupCaption);
                //if (dashboardGroup == null)
                //{
                //    dashboardGroup = new ChoiceActionItem(dashboardOptions.DashboardGroupCaption, null)
                //    {
                //        ImageName = "BO_Folder"
                //    };

                //    items.Add(dashboardGroup);
                //}
                //while (dashboardGroup.Items.Count != 0)
                //{
                //    ChoiceActionItem item = dashboardGroup.Items[0];
                //    dashboardGroup.Items.Remove(item);
                //    actions.Add(item);
                //}
                ////foreach (ChoiceActionItem action in DashboardActions.Keys)
                ////{
                ////    action.Active["HasRights"] = HasRights(action, view);
                ////    actions.Add(action);
                ////}
                //foreach (ChoiceActionItem action in actions.OrderBy(action => action.Model.Index))
                //    dashboardGroup.Items.Add(action);
                ////}
            }
        }


        protected override void OnDeactivated()
        {
            UnsubscribeFromEvents();
            base.OnDeactivated();
        }

        void SubscribeToEvents()
        {
            _showNavigationItemController = Frame.GetController<ShowNavigationItemController>();
            if (_showNavigationItemController != null)
                _showNavigationItemController.ItemsInitialized += ShowNavigationItemControllerItemsInitialized;
            //_showNavigationItemController.CustomInitializeItems += _showNavigationItemController_CustomInitializeItems;
        }


        //public static Dictionary<ChoiceActionItem, DoSoDashboard> DashboardActions
        //{
        //    get { return _dashboardActions ?? (_dashboardActions = new Dictionary<ChoiceActionItem, DoSoDashboard>()); }
        //}

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelOptionsDashboard, IModelOptionsDashboardNavigation>();
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            Frame.GetController<ShowNavigationItemController>().CustomShowNavigationItem += ReportsNavigationController_CustomShowNavigationItem;
            // Perform various tasks depending on the target Window.
        }


    }

    public interface IModelOptionsDashboardNavigation : IModelNode
    {
        [Category("Navigation")]
        [DefaultValue("Dashboards")]
        String DashboardGroupCaption { get; set; }

        [DefaultValue(true)]
        [Category("Navigation")]
        bool DashboardsInGroup { get; set; }
    }
}
