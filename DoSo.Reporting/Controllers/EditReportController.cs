using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DoSo.Reporting.BusinessObjects.Reporting;
using System.IO;
using DevExpress.Spreadsheet;
using DoSoReporting.Module;
using DevExpress.XtraEditors;
using System.Windows.Forms;
using DevExpress.XtraSpreadsheet;

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


            sheetForm.spreadsheetControl1.CellValueChanged += SpreadsheetControl1_CellValueChanged;
            sheetForm.spreadsheetControl1.ActiveWorksheet.Workbook.Options.Events.RaiseOnModificationsViaAPI = true;



            var a = sheetForm.fieldListDockPanel1.Controls[0].Controls[0] as DevExpress.XtraSpreadsheet.SpreadsheetFieldListTreeView;
            a.RepositoryItems.Clear();
            a.ItemDrag += (s, ee) => A_ItemDrag(s, ee, sheetForm);

            sheetForm.Show();
        }

        private void A_ItemDrag(object sender, ItemDragEventArgs e, DoSoSheetFrom form)
        {
            var sheet = form.spreadsheetControl1;
            var aaa = e.Button;
            var activeCell = sheet.ActiveCell;
            var view = sender as DevExpress.XtraSpreadsheet.SpreadsheetFieldListTreeView;
            var selectedItems = view.Selection;

            foreach (var selectedItem in selectedItems)
            {
                var item = selectedItem as DevExpress.XtraReports.Native.DataMemberListNode;
                var a = item.ToString();
            }
            //var item = e.Item as DevExpress.XtraReports.Native.DataMemberListNode;


        }

        private void SpreadsheetControl1_CellValueChanged(object sender, DevExpress.XtraSpreadsheet.SpreadsheetCellEventArgs e)
        {
            if (e.Action != CellValueChangedAction.API)
                return;

            var control = sender as DevExpress.XtraSpreadsheet.SpreadsheetControl;
            control.ActiveWorksheet.Workbook.Options.Events.RaiseOnModificationsViaAPI = false;

            var formulaBeforeSplit = e.Cell.Formula;
            var splitedFormula = System.Text.RegularExpressions.Regex.Split(formulaBeforeSplit, @"&\"" \""&");

            for (int i = 0; i < splitedFormula.Length; i++)
            {
                var text = splitedFormula[i];
                if (!text.StartsWith("="))
                    text = "=" + text;
                control.ActiveWorksheet.Cells[e.Cell.RowIndex, e.Cell.ColumnIndex + i].Formula = text;
            }
            control.ActiveWorksheet.Workbook.Options.Events.RaiseOnModificationsViaAPI = true;
            //var basdf = e;
            //if (!e.Value.IsText)
            //    return;

            //if (e.OldValue != e.Value)
            //    e.Cell.Value = "lkjasdflk";


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
