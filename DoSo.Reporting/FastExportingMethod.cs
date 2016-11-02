using DoSo.Reporting.BusinessObjects;
using Reporting.Reporting.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms.VisualStyles;
using System.Xml.Linq;
using System.Xml.Serialization;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Layout;
using DoSo.Reporting;
using Microsoft.Win32;
using DataTable = System.Data.DataTable;
using DevExpress.Spreadsheet;
using DevExpress.XtraSpreadsheet;

namespace FastExcelExportingDemoCs
{
    public static class FastExportingMethod
    {
        public static string ExecutedQueries;

        public static string ExportData2Excel(ReportDefinition selectedReport, /*SqlConnection sqlConnection*/Session uow, string reportName)
        {
            var sqlQueryCollection = selectedReport.SqlQueryCollection.Where(x => x.ExpiredOn == null).OrderBy(x => x.SheetIndex);
            var ds = new DataSet();
            var list = new List<Tuple<DataTable, SqlQuery>>();

            ExecutedQueries = string.Empty;

            foreach (var query in sqlQueryCollection)
            {
                var commandTextWithParameterValues = query.Query;
                commandTextWithParameterValues = ReplaceParameterValues(selectedReport, commandTextWithParameterValues);
                ExecutedQueries += string.Format("{0}{1}{1}{2}{1}{1}", commandTextWithParameterValues, Environment.NewLine, "---------");

                var data = uow.ExecuteQueryWithMetadata(commandTextWithParameterValues);
                var resultSetColumnNames = data.ResultSet[0];
                var resultSetData = data.ResultSet[1];
                var rows = resultSetData.Rows;
                var dt = new DataTable { TableName = query.Name };

                foreach (var selectStatementResultRow in resultSetColumnNames.Rows)
                {
                    Type type;
                    try
                    {
                        type = Type.GetType("System." + selectStatementResultRow.XmlValues[2]);
                    }
                    catch (Exception)
                    {
                        type = typeof(string);
                    }

                    dt.Columns.Add(selectStatementResultRow.Values[0].ToString(), type);
                }

                foreach (var row in rows)
                {
                    var values = new List<object>();
                    for (var columnIndex = 0; columnIndex < dt.Columns.Count; columnIndex++)
                    {
                        var val = row.Values[columnIndex];
                        if (val is NullValue)
                            val = null;
                        values.Add(val);
                    }
                    dt.Rows.Add(values.ToArray());
                }
                ds.Tables.Add(dt);

                list.Add(new Tuple<DataTable, SqlQuery>(dt, query));
            }

            var newControl = new SpreadsheetControl();
            var template = new SpreadsheetControl();
            //var stream = new FileStream(@"C:\Users\Beka\Desktop\Test.xlsx", FileMode.Open);
            
            template.LoadDocument(@"C:\Users\Beka\Desktop\Test.xlsx");
            newControl.CreateNewDocument();

            for (int i = 0; i < ds.Tables.Count; i++)
            {
                if (i > 0)
                    newControl.Document.Worksheets.Add();
                var worksheet = newControl.Document.Worksheets[i];
                worksheet.CopyFrom(template.Document.Worksheets[i]);
                var range = template.Document.Worksheets[i].GetUsedRange();
                worksheet.Cells[range.TopRowIndex, range.LeftColumnIndex].CopyFrom(range, PasteSpecial.Borders | PasteSpecial.NumberFormats | PasteSpecial.ColumnWidths | PasteSpecial.Formats);

                worksheet.Import(ds.Tables[i], true, 1, 1);

            }

            for (int i = 0; i < template.Document.Worksheets.Count; i++)
            {
                var sheet = template.Document.Worksheets[i];
                var range = sheet.GetDataRange().Where(x => !x.Value.IsEmpty);
                var worksheet = newControl.Document.Worksheets[i];

                foreach (var item in range)
                {
                    worksheet.Cells[item.RowIndex, item.ColumnIndex].Value = item.Value;
                }

                //worksheet.Cells[range.TopRowIndex, range.LeftColumnIndex].CopyFrom(range, PasteSpecial.Values);
                //foreach (var item in range.Where(x => !x.Value.IsEmpty))
                //    worksheet.Cells.CopyFrom(range, PasteSpecial.All);
                //    worksheet.Range.
                //worksheet.InsertCells(range);
                //worksheet.Cells[item.RowIndex, item.ColumnIndex].SetValue(item.Value);
            }

            newControl.ActiveWorksheet.Workbook.SaveDocument(reportName);
            //ExportToExcel(list, path);
            return reportName;
        }


        //public static bool ExportData2Excel(ReportDefinition selectedReport, /*SqlConnection sqlConnection*/UnitOfWork uow, string destination, string reportName)
        //{
        //    try { uow.FindObject<ReportDefinition>(CriteriaOperator.Parse("TRUE")); }
        //    catch (Exception ex) { throw new UserFriendlyException("ბაზასთან კავშირის დამყარება ვერ მოხერხდა"); }

        //    var sqlQueryCollection = selectedReport.SqlQueryCollection.Where(x => x.ExpiredOn == null).OrderBy(x => x.SheetIndex);
        //    var ds = new DataSet();
        //    var list = new List<Tuple<DataTable, SqlQuery>>();
        //    var path = Path.Combine(destination, reportName);

        //    ExecutedQueries = string.Empty;



        //    foreach (var query in sqlQueryCollection)
        //    {
        //        var commandTextWithParameterValues = query.Query;
        //        commandTextWithParameterValues = ReplaceParameterValues(selectedReport, commandTextWithParameterValues);
        //        ExecutedQueries += string.Format("{0}{1}{1}{2}{1}{1}", commandTextWithParameterValues, Environment.NewLine, "---------");

        //        var data = uow.ExecuteQueryWithMetadata(commandTextWithParameterValues);
        //        var resultSetColumnNames = data.ResultSet[0];
        //        var resultSetData = data.ResultSet[1];
        //        var rows = resultSetData.Rows;
        //        var dt = new DataTable { TableName = query.Name };

        //        var prefix = "";
        //        foreach (var selectStatementResultRow in resultSetColumnNames.Rows)
        //        {
        //            Type type;
        //            try { type = Type.GetType("System." + selectStatementResultRow.XmlValues[2]); }
        //            catch (Exception) { type = typeof(string); }
        //            dt.Columns.Add(prefix + selectStatementResultRow.Values[0], type ?? typeof(string));
        //            prefix += "=";
        //        }

        //        foreach (var row in rows)
        //        {
        //            var values = new List<object>();
        //            for (var columnIndex = 0; columnIndex < dt.Columns.Count; columnIndex++)
        //            {
        //                var val = row.Values[columnIndex];
        //                if (val is NullValue)
        //                    val = null;
        //                values.Add(val);
        //            }
        //            dt.Rows.Add(values.ToArray());
        //        }
        //        ds.Tables.Add(dt);

        //        list.Add(new Tuple<DataTable, SqlQuery>(dt, query));
        //    }

        //    if (ds.Tables.OfType<DataTable>().Any(x => x.Rows.Count > 0))
        //    {
        //        throw new Exception("Comment N 6667");
        //        //ExportToExcel(list, path);
        //        return true;
        //    }
        //    else
        //        return false;
        //}

        public static T Deserialize<T>(string xml)
        {
            var xs = new XmlSerializer(typeof(T));
            return (T)xs.Deserialize(new StringReader(xml));
        }

        //public static StringWriter SerializeList(object o)
        //{
        //    var ns = new XmlSerializerNamespaces();
        //    ns.Add("", "");
        //    var xs = new XmlSerializer(o.GetType());
        //    var xml = new StringWriter();
        //    xs.Serialize(xml, o, ns);

        //    return xml;
        //}

        public static string Serialize(object object2Serialize)
        {
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            var serializer = new XmlSerializer(object2Serialize.GetType());
            var sw = new StringWriter();
            serializer.Serialize(sw, object2Serialize, ns);
            return sw.ToString();
        }

        //public static bool ExportData2ExcelFromPostgre(ReportDefinition selectedReport, /*SqlConnection sqlConnection,*/ string destination, string reportName)
        //{
        //    var sqlQueryCollection = selectedReport.SqlQueryCollection;
        //    var ds = new DataSet();
        //    var list = new List<Tuple<System.Data.DataTable, SqlQuery>>();

        //    foreach (var query in sqlQueryCollection)
        //    {
        //        var commandTextWithParameterValues = query.Query;

        //        commandTextWithParameterValues = ReplaceParameterValues(selectedReport, commandTextWithParameterValues);

        //        var dt = new DataTable { TableName = query.Name };
        //        //var command = new SqlCommand(commandTextWithParameterValues, sqlConnection);
        //        var postageConnectionString = "Server=10.1.120.62;User ID=admin;Password=asdfqwer1234;Database=InsuranceCore";
        //        var postageConnection = new NpgsqlConnection(postageConnectionString);

        //        var sqlDataAdapter = new SqlDataAdapter(command);
        //        sqlDataAdapter.Fill(dt);

        //        ds.Tables.Add(dt);
        //        list.Add(new Tuple<DataTable, SqlQuery>(dt, query));
        //    }

        //    if (ds.Tables.OfType<DataTable>().Any(x => x.Rows.Count > 0))
        //    {
        //        var path = Path.Combine(destination, reportName);
        //        ExportToExcel(list, path);
        //        return true;
        //    }
        //    else
        //        return false;
        //}


        public static string ReplaceParameterValues(ReportDefinition selectedReport, string commandTextWithParameterValues)
        {
            foreach (var parameter in selectedReport.QueryParametersCollection)
            {
                var parameterName = parameter.ParameterName;
                var parameterValueAssembled = "";

                if (parameter.ParameterValue == null || (parameter.Value).Trim() == "")
                    parameterValueAssembled = parameter.DefaultValue;
                else
                {
                    var parameterValues = (parameter.Value).Replace("\r\n", ",").Replace("\t", ",").Replace(")", ",").Split(',')
                            .Select(s => s.Trim())
                            .Where(s => s != "")
                            .ToArray();

                    if (parameter.DataType == 0)
                    { throw new Exception("ერთ-ერთი პარამეტრის ტიპი არ არის მითითებული, მიმართეთ სისტემის ადმინისტრატორს."); }

                    for (var i = 0; i < parameterValues.Length; i++)
                    {
                        switch (parameter.DataType)
                        {
                            case DataTypeEnnum.Text:
                                parameterValues[i] = $"N'{parameterValues[i]}'";
                                break;
                            case DataTypeEnnum.Enum:
                                try
                                {
                                    var enumType = Type.GetType(parameter.EnumType.Name);
                                    if (enumType != null)
                                    {
                                        var enumValue = Convert.ToInt32(Enum.Parse(enumType, parameterValues[i]));
                                        parameterValues[i] = $"{enumValue}";
                                    }
                                }
                                catch (Exception)
                                { throw new InvalidCastException($"Unable To Get Value From Enum Parameter {parameterName} - {parameterValues[i]}"); }

                                break;
                            case DataTypeEnnum.Datetime:
                                var value = Convert.ToDateTime(parameter.Value).ToString(HS.ReportCustomDateTimeFormat);
                                parameterValues[i] = $"'{value}'";
                                break;
                            case DataTypeEnnum.Integer:
                                parameterValues[i] = $"{parameterValues[i]}";
                                break;
                            default:
                                parameterValues[i] = $"'{parameterValues[i]}'";
                                break;
                        }


                        //if (parameter.DataType == DataTypeEnnum.Text)
                        //{
                        //    parameterValues[i] = $"N'{parameterValues[i]}'";
                        //    continue;
                        //}
                        //if (parameter.DataType == DataTypeEnnum.Datetime)
                        //{
                        //    try
                        //    {
                        //        var dateValue = Convert.ToDateTime(parameter.Value, CultureInfo.CurrentCulture).ToString("yyyy.MM.dd HH:mm");
                        //        parameterValues[i] = $"'{dateValue}'";
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        throw new Exception(parameter.Value + Environment.NewLine + ex);
                        //    }
                        //}
                        //else
                        //    parameterValues[i] = $"'{parameterValues[i]}'";
                    }
                    parameterValueAssembled = string.Join(",", parameterValues);

                    if (parameterValueAssembled.Contains("'*'"))
                        parameterValueAssembled = null;
                }
                //commandTextWithParameterValues = commandTextWithParameterValues.Replace(parameterName, parameterValueAssembled.IsNull("Null"));
                commandTextWithParameterValues = Regex.Replace(commandTextWithParameterValues, parameterName, parameterValueAssembled ?? "Null", RegexOptions.IgnoreCase);

            }

            return commandTextWithParameterValues;
        }



        static List<ExistingData> GetExistingData(string xml)
        {
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(xml)))
            {
                var serializer = new XmlSerializer(typeof(List<ExistingData>));
                return (List<ExistingData>)serializer.Deserialize(ms);
            }

            //var sheetsCount = xlWorkBook.Worksheets.Count;

            //string xmlText = "";

            //for (int i = 0; i < sheetsCount; i++)
            //{
            //    var xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.Item[i + 1];
            //    var range = xlWorkSheet.UsedRange;

            //    for (int rCnt = 0; rCnt <= range.Rows.Count; rCnt++)
            //    {
            //        for (int cCnt = 0; cCnt <= range.Columns.Count; cCnt++)
            //        {
            //            var singleItem = range.Cells[rCnt + 1, cCnt + 1] as Range;
            //            if (singleItem == null)
            //                continue;

            //            var value = (singleItem.Formula ?? singleItem.Value2)?.ToString();
            //            if (!string.IsNullOrEmpty(value))
            //            {
            //                //existingDatas.Add(new ExistingData(i, singleItem.Row, singleItem.Column, value));
            //            }
            //        }
            //    }
            //}
        }


        //public static void ExportToExcel(List<Tuple<DataTable, SqlQuery>> list, string outputPath)
        //{
        //    //////excel Problem
        //    Application excelApp = new Application();
        //    Workbook excelWorkbook = null;
        //    var reportDefinition = list.FirstOrDefault()?.Item2?.ReportDefinition;
        //    var template = reportDefinition.Template;
        //    var tempPath = string.Empty;
        //    List<ExistingData> existingDatas = null;
        //    List<object> sheets = new List<object>();
        //    //var cc = CultureInfo.CurrentCulture.DateTimeFormat;

        //    try
        //    {
        //        if (!string.IsNullOrEmpty(reportDefinition.TemplateData))
        //        {
        //            var tempName = Path.GetTempFileName() + @".Xlsx";
        //            tempPath = Path.Combine(Path.GetTempPath(), tempName);

        //            using (var stream = new FileStream(tempPath, FileMode.Create))
        //                template.SaveToStream(stream);

        //            object o = Missing.Value;
        //            excelWorkbook = excelApp.Workbooks.Open(tempPath, o, o, o, o, o, o, o, o, o, o, o, o, o, o);
        //            existingDatas = GetExistingData(reportDefinition.TemplateData);
        //        }
        //        else
        //            excelWorkbook = excelApp.Workbooks.Add(Type.Missing);

        //        foreach (var data in list)
        //        {
        //            var dt = data.Item1;
        //            var query = data.Item2;

        //            var rawData = new object[dt.Rows.Count + 1, dt.Columns.Count];
        //            var finalColLetter = string.Empty;
        //            var colCharset = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        //            var colCharsetLen = colCharset.Length;

        //            if (dt.Columns.Count > colCharsetLen)
        //                finalColLetter = colCharset.Substring((dt.Columns.Count - 1) / colCharsetLen - 1, 1);

        //            var startColumnIndex = 0;
        //            var startRowIndex = 1;
        //            var range = "A1";

        //            if (!string.IsNullOrEmpty(query.Range))
        //            {
        //                var firstChar = query.Range.Substring(0, 1);
        //                if (firstChar != "A")
        //                    startColumnIndex = colCharset.IndexOf(firstChar);
        //                startRowIndex = Convert.ToInt32(query.Range.Substring(1));
        //                range = query.Range;
        //            }

        //            for (var col = 0; col < dt.Columns.Count; col++)
        //            {
        //                var existingData =
        //                    existingDatas?.FirstOrDefault(
        //                        x =>
        //                            x.SheetIndex == query.SheetIndex && x.Row == startRowIndex &&
        //                            x.Column == col + startColumnIndex + 1)?.Value;
        //                if (existingData != null)
        //                {
        //                    rawData[0, col] = existingData;
        //                    continue;
        //                }
        //                rawData[0, col] = dt.Columns[col].Caption.Replace("=", "");
        //            }

        //            finalColLetter += colCharset.Substring((dt.Columns.Count + startColumnIndex - 1) % colCharsetLen, 1);

        //            for (var col = 0; col < dt.Columns.Count; col++)
        //            {
        //                for (var row = 0; row < dt.Rows.Count; row++)
        //                {
        //                    var existingData =
        //                        existingDatas?.FirstOrDefault(
        //                            x =>
        //                                x.SheetIndex == query.SheetIndex && x.Row == row + startRowIndex + 1 &&
        //                                x.Column == col + startColumnIndex + 1)?.Value;
        //                    if (existingData != null)
        //                    {
        //                        rawData[row + 1, col] = existingData;
        //                        continue;
        //                    }
        //                    var value = dt.Rows[row].ItemArray[col];
        //                    var value2Assign = value.ToString();
        //                    var dateFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
        //                    if (value is DateTime)
        //                        value2Assign = Convert.ToDateTime(value2Assign).ToString($"{dateFormat} HH:mm");

        //                    value2Assign = value2Assign.Replace("{", "").Replace("}", "");
        //                    value2Assign = value2Assign.Replace("=", "");
        //                    rawData[row + 1, col] = value2Assign.Replace("{", "").Replace("}", "");
        //                }
        //            }

        //            Worksheet excelSheet = null;

        //            if (template != null)
        //                try
        //                {
        //                    excelSheet = (Worksheet)excelWorkbook.Sheets.get_Item(data.Item2.SheetIndex + 1);
        //                }
        //                catch (Exception)
        //                {
        //                }

        //            if (excelSheet == null)
        //                excelSheet = (Worksheet)excelWorkbook.Sheets.Add(
        //                    excelWorkbook.Sheets.get_Item(excelWorkbook.Sheets.Count), Type.Missing, 1,
        //                    XlSheetType.xlWorksheet);

        //            sheets.Add(excelSheet);

        //            foreach (var header in query.ReportHEadersCollection.WhereExpiredOnIsNull())
        //            {
        //                var parameters = query.ReportDefinition.QueryParametersCollection.Where();
        //                var text = header.Text;

        //                foreach (var param in parameters)
        //                    text = text.Replace(param.ParameterName, param.Value);

        //                var ranges = header.Range.Split(':').Where(x => x.Length > 1);
        //                excelSheet.get_Range(ranges.FirstOrDefault(), Type.Missing).Value2 = text;
        //                excelSheet.get_Range(ranges.FirstOrDefault(), ranges.LastOrDefault()).Merge(Type.Missing);
        //            }

        //            var excelRange = $"{range}:{finalColLetter}{dt.Rows.Count + startRowIndex}";

        //            excelSheet.get_Range(excelRange, Type.Missing).Value2 = rawData;
        //            //excelSheet.Application.ActiveWindow.SplitRow = startRowIndex;

        //            if (template == null)
        //            {
        //                try
        //                {
        //                    for (var col = 0; col < dt.Columns.Count; col++)
        //                    {
        //                        var columnType = dt.Columns[col].DataType;

        //                        var rng = (Range)excelSheet.Cells[1, col + 1];

        //                        var columnFormat = "";
        //                        switch (columnType.Name)
        //                        {
        //                            case "String":
        //                                columnFormat = "@";
        //                                break;
        //                            case "DateTime":
        //                                columnFormat = "[$-809]dd MM yyyy;@";
        //                                //columnFormat = "dd-MM-yyyy HH:MM";
        //                                break;
        //                            case "Integer":
        //                                columnFormat = "";
        //                                break;
        //                            //case "Decimal":
        //                            //    columnFormat = "";
        //                            //    break;
        //                            default:
        //                                break;
        //                        }

        //                        rng.EntireColumn.NumberFormat = columnFormat;
        //                    }

        //                    excelSheet.Name = dt.TableName;
        //                    excelSheet.Cells.Font.Size = 8;
        //                    excelSheet.Activate();
        //                    excelSheet.Application.ActiveWindow.SplitRow = 1;
        //                    excelSheet.Application.ActiveWindow.FreezePanes = true;
        //                    var firstRow = excelSheet.Range[range, finalColLetter + startRowIndex];
        //                    firstRow.Activate();
        //                    firstRow.Select();
        //                    firstRow.Font.Bold = true;
        //                    firstRow.AutoFilter(1, Type.Missing, XlAutoFilterOperator.xlAnd, Type.Missing, true);
        //                }
        //                catch (Exception) {/*Ignored*/}
        //            }

        //            var autofitExcelRangeAddress = $"{range}:{finalColLetter}{200}";
        //            var autofitExcelRange = excelSheet.get_Range(autofitExcelRangeAddress);
        //            autofitExcelRange.Columns.AutoFit();

        //            foreach (Range column in autofitExcelRange.Columns)
        //            {
        //                if (column.ColumnWidth > 70)
        //                    column.ColumnWidth = 70;
        //            }
        //        }

        //        excelWorkbook.SaveAs(outputPath, XlFileFormat.xlOpenXMLWorkbook, Type.Missing, Type.Missing, Type.Missing,
        //            Type.Missing, XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //    finally
        //    {
        //        try
        //        {
        //            foreach (var sheet in sheets)
        //                Marshal.FinalReleaseComObject(sheet);

        //            if (excelWorkbook != null)
        //            {
        //                excelWorkbook.Close(true, Type.Missing, Type.Missing);
        //                Marshal.FinalReleaseComObject(excelWorkbook);
        //            }
        //            if (excelApp != null)
        //            {
        //                excelApp.Quit();
        //                Marshal.FinalReleaseComObject(excelApp);
        //            }
        //        }
        //        catch (Exception) {/*ignored*/}

        //        //GC.Collect();
        //        //GC.WaitForPendingFinalizers();

        //        if (!string.IsNullOrEmpty(tempPath))
        //            if (File.Exists(tempPath))
        //                File.Delete(tempPath);
        //    }
        //}
    }
}

[XmlType("DataExportRequest")]
//[FallbackRoute("/")]
public class DataExportRequest
{
    //[XmlElement("ParametersXml")]
    //public string ParametersXml { get; set; }
    [XmlElement("ReportDefinitionID")]
    public int ReportDefinitionID { get; set; }
    [XmlElement("Parameters")]
    public List<QueryParameterHelper> Parameters { get; set; }
    //[DataMember(Name = "Data")]
    //public List<Tuple<DataTable, SqlQuery>> Data { get; set; }
}

public class QueryParameterHelper
{
    [XmlElement("ParameterName")]
    public string ParameterName { get; set; }
    [XmlElement("ParameterValue")]
    public string ParameterValue { get; set; }
}

public class ExistingData
{
    public ExistingData() { }

    //public ExistingData(int sheetIndex, int row, int column, string value)
    //{
    //    SheetIndex = sheetIndex;
    //    Row = row;
    //    Column = column;
    //    Value = value;
    //}

    public int SheetIndex;
    public int Row;
    public int Column;
    public string Value;
}

