using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.XtraReports.UserDesigner;
using DoSo.Reporting.BusinessObjects;
using System.ComponentModel.Design;
using DevExpress.XtraReports.Design.Commands;
using System.Xml;
using System.IO;
using DevExpress.XtraEditors;
using System.Windows.Forms;

namespace DoSo.Reporting.Controllers
{
    public partial class AddReportDataToScheduleController : ObjectViewController<DetailView, DoSoReportSchedule>
    {
        public AddReportDataToScheduleController()
        {
            InitializeComponent();
        }

        private void simpleAction_AddReportData_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var form = new XRDesignRibbonFormEx();
            form.DesignPanel.DesignerHostLoaded += DesignPanel_DesignerHostLoaded;
            ViewCurrentObject.CreateDataSourceFromXml();
            var xml = ViewCurrentObject.ReportDataXml;
            if (string.IsNullOrEmpty(xml))
            {
                form.DesignPanel.ExecCommand(ReportCommand.NewReport);
                var result = form.ShowDialog();
            }
            else
            {
                var report = new DevExpress.XtraReports.UI.XtraReport();
                ViewCurrentObject.AddDataSource2Report(report);

                using (var ms = new MemoryStream())
                {
                    using (var sr = new StreamWriter(ms, Encoding.Default))
                    {
                        var doc = new XmlDocument();
                        doc.LoadXml(xml);
                        var definitionXml = doc.OuterXml;
                        sr.Write(definitionXml);
                        sr.Flush();
                        ms.Position = 0;
                        report.LoadLayoutFromXml(ms);
                        form.DesignPanel.OpenReport(report);
                        form.ShowDialog();
                    }
                }
            }
        }

        private void DesignPanel_DesignerHostLoaded(object sender, DesignerLoadedEventArgs e)
        {
            ViewCurrentObject.CreateDataSourceFromXml();
            var panel = sender as XRDesignPanel;

            panel.AddCommandHandler(new SaveCommandHandler(panel, ViewCurrentObject, ObjectSpace));

            var report = panel.Report;
            report.DesignerLoaded += Report_DesignerLoaded;
            report.DataSource = ViewCurrentObject.SqlDataSource.Result[0];
        }

        private void Report_DesignerLoaded(object sender, DesignerLoadedEventArgs e)
        {
            IMenuCommandService CommandServ = e.DesignerHost.GetService(typeof(IMenuCommandService)) as IMenuCommandService;

            MenuCommand cmd = CommandServ.FindCommand(UICommands.OpenFile);
            if (cmd != null)
                CommandServ.RemoveCommand(cmd);
        }
    }


    public class SaveCommandHandler : ICommandHandler
    {
        XRDesignPanel panel;
        DoSoReportSchedule schedule;
        IObjectSpace os;

        public SaveCommandHandler(XRDesignPanel panel, DoSoReportSchedule schedule, IObjectSpace os)
        {
            this.panel = panel;
            this.schedule = schedule;
            this.os = os;
        }

        void Save(ReportCommand command)
        {
            if (panel.ReportState == ReportState.Changed)
            {
                if (command == ReportCommand.Closing)
                {
                    var result = XtraMessageBox.Show("Do you want save changes?", "Save?", MessageBoxButtons.YesNo);
                    if (result != DialogResult.Yes)
                    {
                        panel.ReportState = ReportState.Saved;
                        return;
                    }
                }
                using (var ms = new MemoryStream())
                {
                    panel.Report.SaveLayoutToXml(ms);
                    ms.Position = 0;
                    using (var sr = new StreamReader(ms, Encoding.Default))
                    {
                        var xml = sr.ReadToEnd();
                        schedule.ReportDataXml = xml;
                        os.CommitChanges();
                    }
                }
            }
            
            // Prevent the "Report has been changed" dialog from being shown.
            panel.ReportState = ReportState.Saved;
        }

        public void HandleCommand(ReportCommand command, object[] args)
        {
            bool b = true;
            if (!CanHandleCommand(command, ref b)) return;
            Save(command);
        }

        public bool CanHandleCommand(ReportCommand command, ref bool useNextHandler)
        {
            if (command == ReportCommand.SaveFile || command == ReportCommand.SaveFileAs || command == ReportCommand.Closing)
            {
                useNextHandler = false;
                return true;
            }

            return false;
        }
    }
}
