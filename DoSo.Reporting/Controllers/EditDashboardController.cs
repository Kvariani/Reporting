using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DoSo.Reporting.BusinessObjects;
using System.IO;
using System.Windows.Forms;
using DevExpress.DashboardWin;
using System.Xml;

namespace DoSo.Reporting.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class EditDashboardController : ViewController
    {
        public EditDashboardController()
        {
            InitializeComponent();
            TargetObjectType = typeof(DoSoDashboard);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();

  
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        private void simpleAction_EditDashboard_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var dashboard = View.CurrentObject as DoSoDashboard;
            if (dashboard != null)
            {
                var form = new DashboardDesignerForm();
                if (string.IsNullOrWhiteSpace(dashboard.Xml))
                    dashboard.LoadDashboardDesignerFromXml(form);
                form.ShowDialog();
            }
        }

        private void simpleAction_PreviwDashboard_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            //var os = Application.CreateObjectSpace();
            //var dashboard = os.GetObject(View.CurrentObject as DoSoDashboard);
            //var detailView = Application.CreateDetailView(os, dashboard);

            //var svp = new ShowViewParameters
            //{
            //    CreatedView = detailView,
            //    NewWindowTarget = NewWindowTarget.Default,
            //    TargetWindow = TargetWindow.Default,
            //};

            using (var viewver = new DashboardViewer())
            {
                viewver.Dock = System.Windows.Forms.DockStyle.Fill;
                using (var ms = new MemoryStream())
                {
                    using (var sr = new StreamWriter(ms, Encoding.Default))
                    {
                        var doc = new XmlDocument();
                        doc.LoadXml((View.CurrentObject as DoSoDashboard).Xml);
                        var definitionXml = doc.OuterXml;
                        sr.Write(definitionXml);
                        sr.Flush();
                        ms.Position = 0;
                        viewver.LoadDashboard(ms);
                        using (var form = new DevExpress.XtraEditors.XtraForm())
                        {
                            form.Width = 1600;
                            form.Height = 900;
                            viewver.Parent = form;
                            form.ShowDialog();
                        }
                    }
                }
            }

            

            //Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));
        }
    }
}
