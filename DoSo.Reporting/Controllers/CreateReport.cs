using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Spreadsheet;
using DevExpress.Xpo;
using DevExpress.XtraSpreadsheet;
using DoSo.Reporting.BusinessObjects;
using DoSo.Reporting.BusinessObjects.Reporting;
using System;
using System.IO;
using System.Linq;

namespace DoSo.Reporting.Controllers
{

    public partial class CreateReport : WindowController
    {
        public CreateReport()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetWindowType = WindowType.Main;
        }

        private void NewReport_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            MaybeFast();

            //var newReport = objectSpace.CreateObject<ReportExecution>();
            //e.ShowViewParameters.NewWindowTarget = NewWindowTarget.Separate;
            //e.ShowViewParameters.Context = TemplateContext.PopupWindow;
            //e.ShowViewParameters.CreatedView = Application.CreateDetailView(objectSpace, newReport, true);
        }


        public void MaybeFast()
        {
            var objectSpace = Application.CreateObjectSpace() as XPObjectSpace;
            var report = objectSpace.Session.Query<DoSoReport>().FirstOrDefault();

            var xml = report?.Xml;
            if (!string.IsNullOrEmpty(xml))
            {
                using (var control = new SpreadsheetControl())
                {
                    using (var ms = new MemoryStream(Convert.FromBase64String(xml)))
                        control.LoadDocument(ms, DocumentFormat.OpenXml);


                    var start = DateTime.Now;

                    var ds = control.Document.MailMergeDataSource as DevExpress.DataAccess.Sql.SqlDataSource;
                    ds.Fill();
                    control.ActiveWorksheet.Import(ds.Result[0], 0, 0);

                    var fullName = Path.Combine(@"C:\Users\Beka\Desktop\New folder", HS.MyTempName + ".Xlsx");
                    control.ActiveWorksheet.Workbook.SaveDocument(fullName);

                    var b = $"{start} <> {DateTime.Now}";
                    var a = start;
                }
            }

            //var template = new SpreadsheetControl();
            ////var stream = new FileStream(@"C:\Users\Beka\Desktop\Test.xlsx", FileMode.Open);

            //var newControl = new SpreadsheetControl();
            //template.LoadDocument(@"C:\Users\Beka\Desktop\Test.xlsx");
            //newControl.CreateNewDocument();

            //for (int i = 0; i < ds.Tables.Count; i++)
            //{
            //    if (i > 0)
            //        newControl.Document.Worksheets.Add();
            //    var worksheet = newControl.Document.Worksheets[i];
            //    worksheet.CopyFrom(template.Document.Worksheets[i]);
            //    var range = template.Document.Worksheets[i].GetUsedRange();
            //    worksheet.Cells[range.TopRowIndex, range.LeftColumnIndex].CopyFrom(range, PasteSpecial.Borders | PasteSpecial.NumberFormats | PasteSpecial.ColumnWidths | PasteSpecial.Formats);

            //    worksheet.Import(ds.Tables[i], true, 1, 1);

            //}

            //for (int i = 0; i < template.Document.Worksheets.Count; i++)
            //{
            //    var sheet = template.Document.Worksheets[i];
            //    var range = sheet.GetDataRange().Where(x => !x.Value.IsEmpty);
            //    var worksheet = newControl.Document.Worksheets[i];

            //    foreach (var item in range)
            //    {
            //        worksheet.Cells[item.RowIndex, item.ColumnIndex].Value = item.Value;
            //    }

            //    //worksheet.Cells[range.TopRowIndex, range.LeftColumnIndex].CopyFrom(range, PasteSpecial.Values);
            //    //foreach (var item in range.Where(x => !x.Value.IsEmpty))
            //    //    worksheet.Cells.CopyFrom(range, PasteSpecial.All);
            //    //    worksheet.Range.
            //    //worksheet.InsertCells(range);
            //    //worksheet.Cells[item.RowIndex, item.ColumnIndex].SetValue(item.Value);
            //}

            //newControl.ActiveWorksheet.Workbook.SaveDocument(reportName);
            ////ExportToExcel(list, path);
            //return reportName;
        }

        public void Slow()
        {
            var objectSpace = Application.CreateObjectSpace() as XPObjectSpace;
            var report = objectSpace.Session.Query<DoSoReport>().FirstOrDefault();

            var xml = report?.Xml;
            if (!string.IsNullOrEmpty(xml))
            {
                using (var control = new SpreadsheetControl())
                {
                    using (var ms = new MemoryStream(Convert.FromBase64String(xml)))
                        control.LoadDocument(ms, DocumentFormat.OpenXml);

                    if (control.Document.MailMergeParameters.Any())
                        throw new InvalidOperationException("Need Parameters");

                    var start = DateTime.Now;

                    var docs = control.Document.GenerateMailMergeDocuments();
                    foreach (var doc in docs)
                    {
                        var fullName = Path.Combine(@"C:\Users\Beka\Desktop\New folder", HS.MyTempName + ".Xlsx");
                        doc.SaveDocument(fullName);
                    }
                    var b = $"{start} <> {DateTime.Now}";
                    var a = start;
                }
            }
        }
    }
}