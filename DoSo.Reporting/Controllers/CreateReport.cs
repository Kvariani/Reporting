using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
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
            //Slow();

            var os = Application.CreateObjectSpace() as XPObjectSpace;
            var view = Application.CreateDetailView(os, new ReportExecution(os.Session));
            var svp = new ShowViewParameters
            {
                CreatedView = view,
                NewWindowTarget = NewWindowTarget.Default,
                TargetWindow = TargetWindow.NewModalWindow,
                Context = TemplateContext.PopupWindow
            };
            var dialogController = new DialogController();
            svp.Controllers.Add(dialogController);
            dialogController.AcceptAction.Execute += (s, ee) => AcceptAction_Execute(s, ee, view);
            Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));





            //var newReport = objectSpace.CreateObject<ReportExecution>();
            //e.ShowViewParameters.NewWindowTarget = NewWindowTarget.Separate;
            //e.ShowViewParameters.Context = TemplateContext.PopupWindow;
            //e.ShowViewParameters.CreatedView = Application.CreateDetailView(objectSpace, newReport, true);
        }

        private void AcceptAction_Execute(object sender, SimpleActionExecuteEventArgs e, DetailView view)
        {
            MaybeFast(e.CurrentObject as ReportExecution, view);
        }

        public void MaybeFast(ReportExecution reportExecution, DetailView view)
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
                    var parameters = ds.Queries.SelectMany(x => x.Parameters);
                    foreach (var item in parameters)
                    {
                        var editorWithValue = view.FindItem(item.Name);

                    }
                    ds.Fill();
                    // Mail Merge details
                    var outDocument = new Workbook();
                    outDocument.Worksheets.Add(HS.MyTempName);
                    outDocument.Worksheets.RemoveAt(0);
                    foreach (var item in control.Document.Worksheets)
                    {
                        control.Document.Worksheets.ActiveWorksheet = item;
                        if (control.ActiveWorksheet.DefinedNames.Any())
                        {
                            var docs = control.Document.GenerateMailMergeDocuments();
                            foreach (var doc in docs)
                            {
                                foreach (var sheet in doc.Worksheets)
                                {
                                    outDocument.Worksheets.Add(sheet.Name);
                                    outDocument.Worksheets.LastOrDefault().CopyFrom(sheet);
                                }
                            }
                        }
                        else
                        {
                            outDocument.Worksheets.Add(item.Name);
                            var userdRange = item.GetDataRange().Where(x => x.HasFormula && x.Formula.ToLower().Contains("=field("));
                            if (userdRange.Any())
                            {
                                foreach (var rangeItem in userdRange)
                                {
                                    if (rangeItem.RowIndex > 0)
                                    {
                                        var headerCell = outDocument.Worksheets.LastOrDefault().Cells[rangeItem.RowIndex - 1, rangeItem.ColumnIndex];
                                        if (headerCell.Value.IsEmpty)
                                            headerCell.SetValue(rangeItem.DisplayText);
                                    }
                                    var dataMember = control.Document.MailMergeDataMember;
                                    var splitedItem = rangeItem.DisplayText.Split('.');
                                    if (splitedItem.Count() > 1)
                                        dataMember = splitedItem.FirstOrDefault().Replace("[", "");

                                    var query = ds.Result.Where(x => x.Name == dataMember).SelectMany(x => x.Columns).Where(x => x.Name == splitedItem.LastOrDefault().Replace("]", "").Replace("[", "")).FirstOrDefault() as DevExpress.DataAccess.Native.Sql.ResultColumn;

                                    for (int i = 0; i < query.Count; i++)
                                    {
                                        var value = query.Values[i];
                                        var cell = outDocument.Worksheets.LastOrDefault().Cells[rangeItem.RowIndex + i, rangeItem.ColumnIndex];
                                        cell.SetValue(value);
                                    }
                                }
                            }
                            else
                            {
                                var lastIndex = 0;
                                foreach (var result in ds.Result)
                                {
                                    outDocument.Worksheets.LastOrDefault().Import(result, 1, lastIndex);
                                    foreach (var column in result.Columns)
                                    {
                                        var headerCell = outDocument.Worksheets.LastOrDefault().Cells[0, lastIndex];
                                        if (headerCell.Value.IsEmpty)
                                            headerCell.SetValue(column.Name);
                                        lastIndex++;
                                    }
                                }
                            }
                        }
                    }
                    outDocument.Worksheets.RemoveAt(0);

                    var fullName = Path.Combine(@"C:\Users\Beka\Desktop\New folder", HS.MyTempName + ".Xlsx");
                    outDocument.SaveDocument(fullName);
                    return;

                    var sdfa = control.ActiveWorksheet.DefinedNames.Where(x => x == x);
                    control.ActiveWorksheet.Import(ds.Result[0], 0, 0);


                    control.ActiveWorksheet.Workbook.SaveDocument(fullName);

                    var b = $"{start} <> {DateTime.Now}";
                    var a = start;
                }
            }
        }


        //public void Slow()
        //{
        //    var objectSpace = Application.CreateObjectSpace() as XPObjectSpace;
        //    var report = objectSpace.Session.Query<DoSoReport>().FirstOrDefault();

        //    var xml = report?.Xml;
        //    if (!string.IsNullOrEmpty(xml))
        //    {
        //        using (var control = new SpreadsheetControl())
        //        {
        //            using (var ms = new MemoryStream(Convert.FromBase64String(xml)))
        //                control.LoadDocument(ms, DocumentFormat.OpenXml);

        //            if (control.Document.MailMergeParameters.Any())
        //                throw new InvalidOperationException("Need Parameters");

        //            var start = DateTime.Now;

        //            var docs = control.Document.GenerateMailMergeDocuments();

        //            foreach (var item in docs)
        //            {
        //                if (control.Document.Worksheets.Count > 1)
        //                {
        //                    for (int i = 1; i < control.Document.Worksheets.Count; i++)
        //                    {
        //                        control.Document.Worksheets.ActiveWorksheet = control.Document.Worksheets[i];
        //                        var docs2 = control.Document.GenerateMailMergeDocuments();
        //                        foreach (var secondWorkSheet in docs2.SelectMany(x => x.Worksheets))
        //                        {
        //                            item.Worksheets.Add();
        //                            var newSheet = item.Worksheets[i];
        //                            newSheet.CopyFrom(secondWorkSheet);
        //                        }
        //                    }
        //                }
        //            }
        //            foreach (var doc in docs)
        //            {
        //                var fullName = Path.Combine(@"C:\Users\Beka\Desktop\New folder", HS.MyTempName + ".Xlsx");
        //                doc.SaveDocument(fullName);
        //            }
        //            var b = $"{start} <> {DateTime.Now}";
        //            var a = start;
        //        }
        //    }
        //}

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
}