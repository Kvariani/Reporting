using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraPrinting.Export;
using NewBaseModule.BisinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NewBaseModule.Controllers
{
    public class ConfigurationStaticController : WindowController
    {
        public ConfigurationStaticController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetWindowType = WindowType.Main;
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            //var currentUser = SecuritySystem.CurrentUser as SecuritySystemUserExBase;
            //_popupWindowShowActionOpenControlingObject.Active.SetItemValue("IsManager", currentUser?.IsManager ?? false);
        }

        private PopupWindowShowAction _popupWindowShowActionOpenControlingObject;
        private System.ComponentModel.IContainer components;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                components?.Dispose();
            base.Dispose(disposing);
        }
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            _popupWindowShowActionOpenControlingObject = new PopupWindowShowAction(components)
            {
                Caption = "Configuration",
                Id = "popupWindowShowAction_OpenConfigurationStatic",
                ImageName = "OpenConfigurationStatic",
                Category = "Tools",
                ToolTip = null
            };
            _popupWindowShowActionOpenControlingObject.CustomizePopupWindowParams += PopupWindowShowAction_OpenControlingObject_CustomizePopupWindowParams; ;
        }

        private void PopupWindowShowAction_OpenControlingObject_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var os = Application.CreateObjectSpace() as XPObjectSpace;
            var configs = os.Session.Query<ConfigurationStatic>().Where(x => x.ExpiredOn == null).ToList();
            var obj = new Configuration();
            var view = Application.CreateDetailView(os, obj);
            e.DialogController.AcceptAction.Executed += (s, ee) =>
            {
                HS.GetConfiguration();
                os.CommitChanges();
            };
            CreateViewItems(view, configs);
            e.View = view;
        }

        public void CreateViewItems(DetailView view, List<ConfigurationStatic> configurations)
        {
            view.CreateControls();
            var layoutControl = (DevExpress.XtraLayout.LayoutControl)view.Control;
            var mainGroup = new TabbedControlGroup() { TextVisible = false };
            layoutControl.AddItem(mainGroup);

            var groups = new List<LayoutControlGroup>();

            foreach (var configurationStatic in configurations)
            {
                var item = new LayoutControlItem() { Name = configurationStatic.ParameterName, OptionsToolTip = new BaseLayoutItemOptionsToolTip() { ToolTip = configurationStatic.Description } };
                var group = groups.FirstOrDefault(x => x.Name == configurationStatic.GroupName);
                if (group == null)
                {
                    group = new LayoutControlGroup() { Name = configurationStatic.GroupName, DefaultLayoutType = LayoutType.Vertical };
                    groups.Add(group);
                    mainGroup.AddTabPage(group);
                }
                var type = configurationStatic.ParameterType;
                switch (type)
                {
                    case ConfigurationStatic.ParameterTypeEnum.String:
                        item.Control = new StringEdit(250) { Dock = DockStyle.Fill, EditValue = configurationStatic.ParameterValue, ToolTip = configurationStatic.Description }; break;
                    case ConfigurationStatic.ParameterTypeEnum.Int:
                        item.Control = new IntegerEdit() { Dock = DockStyle.Fill, EditValue = Convert.ToInt32(configurationStatic.ParameterValue), ToolTip = configurationStatic.Description }; break;
                    case ConfigurationStatic.ParameterTypeEnum.Decimal:
                        item.Control = new DecimalEdit() { Dock = DockStyle.Fill, EditValue = Convert.ToDecimal(configurationStatic.ParameterValue), ToolTip = configurationStatic.Description }; break;
                    case ConfigurationStatic.ParameterTypeEnum.Bool:
                        item.Control = new BooleanEdit() { Dock = DockStyle.Fill, EditValue = Convert.ToBoolean(configurationStatic.ParameterValue), ToolTip = configurationStatic.Description, Text = string.Empty }; break;
                }

                (item.Control as BaseEdit).EditValueChanged += (s, e) => ConfigurationStaticController_EditValueChanged(s, e, configurationStatic);
                group.AddItem(item);
            }
            mainGroup.SelectedTabPageIndex = 0;
        }

        private void ConfigurationStaticController_EditValueChanged(object sender, EventArgs e, ConfigurationStatic config)
        {
            var edit = sender as BaseEdit;
            var newvalue = edit.EditValue;
            config.ParameterValue = newvalue.ToString();
        }

    }

    [DomainComponent]
    public class Configuration { }
}