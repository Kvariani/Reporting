using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.XtraReports.UI;
using DoSo.Reporting.BusinessObjects.Email;
using System.IO;
using System.Xml;
using DevExpress.LookAndFeel;

namespace DoSo.Reporting.Controllers
{
    public partial class PrevewEmailReportController : ViewController
    {
        public PrevewEmailReportController()
        {
            InitializeComponent();
            TargetObjectType = typeof(DoSoEmail);
        }

        private void simpleAction_PrevewReport_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var email = View.CurrentObject as DoSoEmail;
            var schedule = email?.DoSoReportSchedule;
            var xml = email?.DoSoReportSchedule?.ReportDataXml;
            if (string.IsNullOrWhiteSpace(xml))
                return;

            var report = new XtraReport();

            schedule.CreateDataSourceFromXml();
            schedule.ExcelDataSource?.Fill();
            schedule.SqlDataSource?.Fill();
            schedule.AddDataSource2Report(report);

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
                    report.FilterString = $"{schedule.ObjectKeyExpression} == {email.ObjectKey}";
                    report.ApplyFiltering();
                    //report.FillDataSource();

                    using (ReportPrintTool printTool = new ReportPrintTool(report))
                        printTool.ShowRibbonPreviewDialog(UserLookAndFeel.Default);
                }
            }
        }
    }
}
