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
using DoSo.Reporting.BusinessObjects.Email;
using DevExpress.DashboardWin;
using System.IO;
using System.Xml;


namespace DoSo.Reporting.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class PrevewEmailDashboardController : ObjectViewController<ListView, DoSoEmail>
    {
        public PrevewEmailDashboardController()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }

        private void simpleAction_PrevewDashboard_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            ViewCurrentObject?.DoSoReportSchedule?.GenerateDashboard(ViewCurrentObject, true);
        }

        //public void ExportDashboard()
        //{
        //    var dashboardXml = ViewCurrentObject.DoSoReportSchedule.DashboardXml;


        //    var viewver = new DashboardViewer();
        //    var form = new DevExpress.XtraEditors.XtraForm();
        //    viewver.Parent = form;

        //    viewver.Dock = System.Windows.Forms.DockStyle.Fill;
        //    using (var ms = new MemoryStream())
        //    {
        //        using (var sr = new StreamWriter(ms, Encoding.Default))
        //        {
        //            var doc = new XmlDocument();
        //            doc.LoadXml(dashboardXml);
        //            var definitionXml = doc.OuterXml;
        //            sr.Write(definitionXml);
        //            sr.Flush();
        //            ms.Position = 0;
        //            var dashboar = new Dashboard();
        //            dashboar.LoadFromXml(ms);
        //            dashboar
        //        }
        //    }
        //}
    }
}
