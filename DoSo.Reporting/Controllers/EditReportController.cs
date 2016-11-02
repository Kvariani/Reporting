using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DoSo.Reporting.BusinessObjects.Reporting;
using System.IO;
using DevExpress.Spreadsheet;
using DoSoReporting.Module;
using DevExpress.XtraEditors;
using System.Windows.Forms;

namespace DoSo.Reporting.Controllers
{
    public partial class EditReportController : ViewController
    {
        public EditReportController()
        {
            InitializeComponent();
            TargetObjectType = typeof(DoSoReport);
        }
        private void simpleAction_EditReport_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var report = View.CurrentObject as DoSoReport;
            if (report == null)
                return;
            var sheetForm = DoSoReport.CreateSheetForm();
            sheetForm.FormClosing += SheetForm_FormClosing;

            var xml = report?.Xml;
            if (!string.IsNullOrEmpty(xml))
                using (var ms = new MemoryStream(Convert.FromBase64String(xml)))
                    sheetForm.spreadsheetControl1.LoadDocument(ms, DocumentFormat.OpenXml);
            sheetForm.Show();
        }

        private void SheetForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var result = XtraMessageBox.Show("Do you want save changes?", "Save?", MessageBoxButtons.YesNoCancel);
            var report = View?.CurrentObject as DoSoReport;
            if (result == DialogResult.Yes)
            {
                var form = sender as DoSoSheetFrom;
                var xml = DoSoReport.GetDocumentXml(form.spreadsheetControl1);
                report.Xml = xml;
                ObjectSpace.CommitChanges();
            }
            if (result == DialogResult.Cancel)
                e.Cancel = true;
        }

    }
}
