using NewBaseModule.BisinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpo;
using DevExpress.Persistent.Base;
using DoSoReporting.Module;
using DevExpress.XtraSpreadsheet.Services;
using DoSo.Reporting.Controllers;
using DevExpress.XtraSpreadsheet;
using System.IO;
using DevExpress.Spreadsheet;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Excel;
using DevExpress.DataAccess.Native.Excel;

namespace DoSo.Reporting.BusinessObjects.Reporting
{
    [DefaultClassOptions]
    public class DoSoReport : NewXPLiteObjectEx
    {
        public DoSoReport(Session session) : base(session)
        {
        }

        private string fName;
        public string Name
        {
            get { return fName; }
            set { SetPropertyValue(nameof(Name), ref fName, value); }
        }

        private bool fIsActive;
        public bool IsActive
        {
            get { return fIsActive; }
            set { SetPropertyValue(nameof(IsActive), ref fIsActive, value); }
        }

        private string fXml;
        [Size(SizeAttribute.Unlimited)]
        public string Xml
        {
            get { return fXml; }
            set { SetPropertyValue(nameof(Xml), ref fXml, value); }
        }

        public static DoSoSheetFrom CreateSheetForm()
        {
            var sheetForm = new DoSoSheetFrom(true);

            sheetForm.spreadsheetControl1.Options.DataSourceWizard.EnableCustomSql = true;

            ISpreadsheetCommandFactoryService service = sheetForm.spreadsheetControl1.GetService(typeof(ISpreadsheetCommandFactoryService)) as ISpreadsheetCommandFactoryService;
            sheetForm.spreadsheetControl1.ReplaceService<ISpreadsheetCommandFactoryService>(new CustomCommandFactoryServise(service, sheetForm.spreadsheetControl1));

            return sheetForm;
        }

        public static string GetDocumentXml(SpreadsheetControl control)
        {
            using (var ms = new MemoryStream())
            {
                control.Document.SaveDocument(ms, DocumentFormat.OpenXml);
                ms.Position = 0;
                return Convert.ToBase64String(ms.ToArray());
            }
        }


        public Workbook GetOutDocument()
        {
            var outDocument = new Workbook();
            outDocument.Worksheets.Add(HS.MyTempName);
            outDocument.Worksheets.RemoveAt(0);
            return outDocument;
        }

        public void ExportByMailMerge(SpreadsheetControl control, Workbook outDocument)
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

        public Workbook ExportFromExcelDataSource(ExcelDataSource excelDataSource, SpreadsheetControl control)
        {
            excelDataSource.Fill();
            var outDocument = GetOutDocument();

            foreach (var item in control.Document.Worksheets)
            {
                control.Document.Worksheets.ActiveWorksheet = item;
                if (control.ActiveWorksheet.DefinedNames.Any())
                {
                    ExportByMailMerge(control, outDocument);
                }
                else
                {
                    outDocument.Worksheets.Add(item.Name);
                    outDocument.Worksheets.LastOrDefault().CopyFrom(item);
                    var userdRange = item.GetDataRange().Where(x => x.HasFormula && x.Formula.ToLower().Contains("=field("));
                    if (userdRange.Any())
                    {
                        foreach (var rangeItem in userdRange)
                        {
                            if (rangeItem.RowIndex > 0)
                            {
                                var headerCell = outDocument.Worksheets.LastOrDefault().Cells[rangeItem.RowIndex - 1, rangeItem.ColumnIndex];
                                if (headerCell.Value.IsEmpty)
                                    headerCell.SetValue(rangeItem.DisplayText.Replace("]", "").Replace("[", ""));
                            }
                            var dataMember = control.Document.MailMergeDataMember;
                            var splitedItem = rangeItem.DisplayText.Split('.');
                            if (splitedItem.Count() > 1)
                                dataMember = splitedItem.FirstOrDefault().Replace("[", "");

                            //var query = ds.Result.Where(x => x.Name == dataMember).SelectMany(x => x.Columns).Where(x => x.Name == splitedItem.LastOrDefault().Replace("]", "").Replace("[", "")).FirstOrDefault() as DevExpress.DataAccess.Native.Sql.ResultColumn;

                            //for (int i = 0; i < query.Count; i++)
                            //{
                            //    var value = query.Values[i];
                            //    var cell = outDocument.Worksheets.LastOrDefault().Cells[rangeItem.RowIndex + i, rangeItem.ColumnIndex];
                            //    cell.SetValue(value);
                            //}
                        }
                    }
                    else
                    {
                        var resultView = HS.GetResultView(excelDataSource);
                        for (int i = 0; i < resultView.Columns.Count; i++)
                        {
                            var col = resultView.Columns[i];
                            var doc = outDocument.Worksheets.LastOrDefault();
                            var headerCell = doc.Cells[0, i];
                            if (headerCell.Value.IsEmpty)
                                headerCell.SetValue(col.Name);

                            foreach (ViewRow row in resultView)
                            {
                                var value = col.GetValue(row);
                                doc.Cells[row.Index + 1, i].SetValue(value);
                            }
                        }
                    }
                }
                ReplaceTemplateValues(item, outDocument);
            }
            return outDocument;

        }

        public Workbook ExportFromSqlDataSource(SqlDataSource ds, SpreadsheetControl control)
        {
            ds.Fill();
            var outDocument = GetOutDocument();
            foreach (var item in control.Document.Worksheets)
            {
                control.Document.Worksheets.ActiveWorksheet = item;
                if (control.ActiveWorksheet.DefinedNames.Any())
                    ExportByMailMerge(control, outDocument);
                else
                {
                    outDocument.Worksheets.Add(item.Name);
                    outDocument.Worksheets.LastOrDefault().CopyFrom(item);
                    var userdRange = item.GetDataRange().Where(x => x.HasFormula && x.Formula.ToLower().Contains("=field("));
                    if (userdRange.Any())
                    {
                        foreach (var rangeItem in userdRange)
                        {
                            if (rangeItem.RowIndex > 0)
                            {
                                var headerCell = outDocument.Worksheets.LastOrDefault().Cells[rangeItem.RowIndex - 1, rangeItem.ColumnIndex];
                                if (headerCell.Value.IsEmpty)
                                    headerCell.SetValue(rangeItem.DisplayText.Replace("]", "").Replace("[", ""));
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
                    ReplaceTemplateValues(item, outDocument);
                }
            }
            return outDocument;
        }

        public void ReplaceTemplateValues(Worksheet item, Workbook outDocument)
        {
            var templateValues = item.GetDataRange().Where(x => !x.Formula.ToLower().Contains("=field(") && !string.IsNullOrEmpty(x.DisplayText));
            foreach (var templateValue in templateValues)
            {
                var cell = outDocument.Worksheets.LastOrDefault().Cells[templateValue.RowIndex, templateValue.ColumnIndex];
                var value = templateValue.Value.ToString();
                cell.SetValue(value);
            }
        }

        public void MaybeFast(ReportExecution reportExecution, DetailView view, XafApplication applciation, bool showMeResult = false)
        {
            var objectSpace = applciation.CreateObjectSpace() as XPObjectSpace;
            var report = objectSpace.Session.Query<DoSoReport>().FirstOrDefault();

            var xml = report?.Xml;
            if (!string.IsNullOrEmpty(xml))
            {
                Workbook outDocument = null;
                var control = reportExecution.SpreadsheetControl;
                if (control.Document.MailMergeDataSource is SqlDataSource)
                    outDocument = ExportFromSqlDataSource(control.Document.MailMergeDataSource as SqlDataSource, control);
                if (control.Document.MailMergeDataSource is ExcelDataSource)
                    outDocument = ExportFromExcelDataSource(control.Document.MailMergeDataSource as ExcelDataSource, control);

                outDocument.Worksheets.RemoveAt(0);

                var fullName = Path.Combine(@"C:\Users\Beka\Desktop\New folder", HS.MyTempName + ".Xlsx");
                outDocument.SaveDocument(fullName);

                if (showMeResult)
                    using (var sheetForm = new DoSoSheetFrom(false))
                    {
                        sheetForm.spreadsheetControl1.LoadDocument(fullName);
                        sheetForm.ShowDialog();
                    }
            }
        }

    }
}
