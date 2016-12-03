using DevExpress.DashboardCommon;
using DevExpress.DashboardWin;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Layout;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.XtraLayout;
using DevExpress.XtraSplashScreen;
using DoSo.Reporting.BusinessObjects;
using DoSo.Reporting.BusinessObjects.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoSo.Reporting.Controllers
{
    public class DefaultDashboardViewController : ObjectViewController<DetailView, MainDashboard>
    {
        public DefaultDashboardViewController()
        {
            InitializeComponent();
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            ObjectSpace.Reloaded += ObjectSpace_Reloaded;
        }

        private void ObjectSpace_Reloaded(object sender, EventArgs e)
        {
            viewver?.ReloadData();
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            CreateDashboard();
        }

        private void SimpleAction_EditDashboard_Execute(object sender, DevExpress.ExpressApp.Actions.SimpleActionExecuteEventArgs e)
        {
            using (var uow = new UnitOfWork(XpoDefault.DataLayer))
            {
                var query = $"Delete From {nameof(DoSoExceptionLog)}";
                uow.ExecuteNonQuery(query);
                ObjectSpace.Refresh();
            }
        }

        public void CreateDashboard()
        {
            try
            {
                tryCount = 0;
                var layoutControl = (LayoutControl)View.Control;
                //var editor = layoutControl.Items.OfType<XafLayoutControlGroup>().FirstOrDefault();
                layoutControl.Items.FirstOrDefault().Shown += DefaultDashboardViewController_Shown;
                //editor.Shown += Editor_Shown;
                //try { SplashScreenManager.ShowForm(null); }
                //catch (Exception) {/*Ignored*/ }
            }
            catch (Exception ex) {/*Ignored*/}
        }

        private void DefaultDashboardViewController_Shown(object sender, EventArgs e)
        {
            this.PutDashboardInLayoutGroup(sender as LayoutControlGroup);
        }

        private void Editor_Shown(object sender, EventArgs e)
        {
            this.PutDashboardInLayoutGroup(sender as XafLayoutControlGroup);
        }

        //public void PutDashboardInLayoutGroup(XafLayoutControlGroup editor)
        //{
        //    var os = ObjectSpace as XPObjectSpace;
        //    var session = os?.Session;
        //    var dashboardName = editor.CustomizationFormText.Split('_').LastOrDefault();

        //    var template = session.Query<DashboardDefinition>().FirstOrDefault(x => x.Name.ToLower() == dashboardName.ToLower());
        //    if (template == null)
        //        return;

        //    Dashboard dashBoard = template.CreateDashBoard();
        //    IEnumerable<DashboardParameter> dashboardParameters = dashBoard.Parameters.Where<DashboardParameter>((Func<DashboardParameter, bool>)(x => x.Name.ToLower() == "currentobject"));
        //    XPLiteObjectEx xpLiteObjectEx = this.View.CurrentObject as XPLiteObjectEx;
        //    var cu = (View.CurrentObject as XPLiteObjectEx)?.ID;
        //    foreach (var parameter in dashboardParameters)
        //        parameter.Value = cu.ToString();

        //    DashboardViewer dashboardViewer = new DashboardViewer();
        //    int num1 = 5;
        //    dashboardViewer.Dock = (DockStyle)num1;
        //    DashboardViewer viewver = dashboardViewer;
        //    List<DataSource> dss = new List<DataSource>();
        //    dss.AddRange((IEnumerable<DataSource>)dashBoard.DataSources);
        //    dashBoard.DataSources.Clear();
        //    XafLayoutControlGroup layoutControlGroup = editor;
        //    LayoutControlItem layoutControlItem = new LayoutControlItem();
        //    layoutControlItem.Control = (Control)viewver;
        //    int num2 = 0;
        //    layoutControlItem.TextVisible = num2 != 0;
        //    layoutControlGroup.Add((BaseLayoutItem)layoutControlItem);
        //    viewver.Dashboard = dashBoard;
        //    Task.Run((Action)(() => this.AddDataSource2Dashboard(dss, viewver)));
        //}
        DashboardViewer viewver;
        public void PutDashboardInLayoutGroup(LayoutControlGroup editor)
        {
            var os = ObjectSpace as XPObjectSpace;
            var session = os?.Session;

            var template = session.Query<DoSoDashboard>().FirstOrDefault(x => x.Name.ToLower() == "default");
            if (template == null)
                return;
            var dashboard = template.CreateDashBoard();

            var parameters = dashboard.Parameters.Where(x => x.Name.ToLower() == "currentobject");

            editor.Items.Clear();
            viewver = new DashboardViewer { Dock = DockStyle.Fill };
            

            //var dss = new List<DataSource>();
            ////dss.AddRange(dashboard.DataSources);
            //dashboard.DataSources.Clear();

            editor.Add(new LayoutControlItem() { Control = viewver, TextVisible = false });

            viewver.Dashboard = dashboard;

            //Task.Run(() => AddDataSource2Dashboard(dss, viewver));
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();

            if (SplashScreenManager.Default?.IsSplashFormVisible != null)
                SplashScreenManager.CloseForm();
        }

        private int tryCount;

        public void AddDataSource2Dashboard(List<DataSource> dataSources, DashboardViewer viewver)
        {
            try
            {
                if (tryCount > 6)
                    return;

                viewver.Dashboard.DataSources.AddRange((IEnumerable<DataSource>)dataSources);
                viewver.Dashboard.BeginInit();
                viewver.Dashboard.EndInit();
            }
            catch (Exception ex)
            {
                return;
                tryCount++;
                viewver.Dashboard.DataSources.Clear();
                viewver.Invoke(new MethodInvoker(() => AddDataSource2Dashboard(dataSources, viewver)));

                //ასდფსადფ
                //dfg
            }
        }

        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            simpleAction_ClearExceptions = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            simpleAction_ClearExceptions.Caption = "Clear Exceptions";
            simpleAction_ClearExceptions.ConfirmationMessage = "Are You Sure?";
            simpleAction_ClearExceptions.Id = "simpleAction_ClearExceptions";
            simpleAction_ClearExceptions.ToolTip = null;
            simpleAction_ClearExceptions.Execute += SimpleAction_EditDashboard_Execute;
            Actions.Add(simpleAction_ClearExceptions);
        }

        

        private DevExpress.ExpressApp.Actions.SimpleAction simpleAction_ClearExceptions;
    }
}
