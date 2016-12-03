using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DoSo.Reporting.BusinessObjects;

namespace Common.Win.General.DashBoard.Controllers
{
    public partial class DashboardNavigationController : WindowController, IModelExtender
    {
        static Dictionary<ChoiceActionItem, DoSoDashboard> _dashboardActions;
        ShowNavigationItemController _showNavigationItemController;
        public static bool CurrentUserIsNotAdministrator;

        public DashboardNavigationController()
        {
            TargetWindowType = WindowType.Main;
        }

        public static Dictionary<ChoiceActionItem, DoSoDashboard> DashboardActions
        {
            get { return _dashboardActions ?? (_dashboardActions = new Dictionary<ChoiceActionItem, DoSoDashboard>()); }
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelOptionsDashboard, IModelOptionsDashboardNavigation>();
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

        void _showNavigationItemController_CustomInitializeItems(object sender, HandledEventArgs e)
        {

        }

        void UnsubscribeFromEvents()
        {
            if (_showNavigationItemController != null)
            {
                _showNavigationItemController.ItemsInitialized -= ShowNavigationItemControllerItemsInitialized;
                _showNavigationItemController = null;
            }
        }

        protected override void OnFrameAssigned()
        {
            UnsubscribeFromEvents();
            base.OnFrameAssigned();
            SubscribeToEvents();
        }

        void ShowNavigationItemControllerItemsInitialized(object sender, EventArgs e)
        {
            IModelView view = Application.FindModelView(Application.FindListViewId(typeof(DoSoDashboard)));
            var options = ((IModelOptionsDashboards)Application.Model.Options);
            var dashboardOptions = ((IModelOptionsDashboardNavigation)options.Dashboards);
            if (dashboardOptions.DashboardsInGroup)
            {
                ReloadDashboardActions();
                var actions = new List<ChoiceActionItem>();
                if (DashboardActions.Count > 0)
                {
                    var dashboardGroup = GetGroupFromActions(((ShowNavigationItemController)sender).ShowNavigationItemAction, dashboardOptions.DashboardGroupCaption);
                    if (dashboardGroup == null)
                    {
                        dashboardGroup = new ChoiceActionItem(dashboardOptions.DashboardGroupCaption, null)
                        {
                            ImageName = "BO_DashboardDefinition"
                        };
                        var items = ((ShowNavigationItemController)sender).ShowNavigationItemAction.Items;
                        items.Add(dashboardGroup);
                    }
                    while (dashboardGroup.Items.Count != 0)
                    {
                        ChoiceActionItem item = dashboardGroup.Items[0];
                        dashboardGroup.Items.Remove(item);
                        actions.Add(item);
                    }
                    foreach (ChoiceActionItem action in DashboardActions.Keys)
                    {
                        action.Active["HasRights"] = HasRights(action, view);
                        actions.Add(action);
                    }
                    foreach (ChoiceActionItem action in actions.OrderBy(action => action.Model.Index))
                        dashboardGroup.Items.Add(action);
                }
            }
        }

        protected virtual bool HasRights(ChoiceActionItem item, IModelView view)
        {
            var data = (ViewShortcut)item.Data;
            if (view == null)
            {
                throw new ArgumentException(string.Format("Cannot find the '{0}' view specified by the shortcut: {1}",
                                                          data.ViewId, data));
            }
            Type type = (view is IModelObjectView) ? ((IModelObjectView)view).ModelClass.TypeInfo.Type : null;
            if (type != null)
            {
                if (!string.IsNullOrEmpty(data.ObjectKey) && !data.ObjectKey.StartsWith("@"))
                {
                    try
                    {
                        using (IObjectSpace space = CreateObjectSpace())
                        {
                            object objectByKey = space.GetObjectByKey(type, space.GetObjectKey(type, data.ObjectKey));
                            return (DataManipulationRight.CanRead(type, null, objectByKey, null, space) &&
                                    DataManipulationRight.CanNavigate(type, objectByKey, space));
                        }
                    }
                    catch
                    {
                        return true;
                    }
                }
                return DataManipulationRight.CanNavigate(type, null, null);
            }
            return true;
        }

        protected virtual IObjectSpace CreateObjectSpace()
        {
            return Application.CreateObjectSpace();
        }

        public virtual void UpdateNavigationImages()
        {
        }


        public virtual void ReloadDashboardActions()
        {
            if (!CurrentUserIsNotAdministrator)
            {
                DashboardActions.Clear();
                IObjectSpace objectSpace = Application.CreateObjectSpace();
                IOrderedEnumerable<DoSoDashboard> templates =
                    objectSpace.GetObjects<DoSoDashboard>().Where(t => t.VisibleInNavigation).OrderBy(i => i.Index);
                foreach (DoSoDashboard template in templates)
                {
                    var action = new ChoiceActionItem(template.ID.ToString(),template.Name, new ViewShortcut("DashboardViewer_DetailView", template.ID.ToString()))
                    {
                        ImageName = "BO_DashboardDefinition"
                    };
                    action.Model.Index = template.Index;
                    DashboardActions.Add(action, template);
                }
            }
        }

        public void RecreateNavigationItems()
        {
            _showNavigationItemController.RecreateNavigationItems();
        }

        public ChoiceActionItem GetGroupFromActions(SingleChoiceAction action, String name)
        {
            return action.Items.FirstOrDefault(item => item.Caption.Equals(name));
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
