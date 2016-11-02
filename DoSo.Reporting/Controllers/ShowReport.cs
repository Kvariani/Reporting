using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Xpo;
using DoSo.Reporting.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Microsoft.Win32;
using NewBaseModule.BisinessObjects;

namespace DoSo.Reporting.Controllers
{
    public partial class ShowReport : ViewController
    {
        public ShowReport()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetObjectType = typeof(ReportExecution);
        }

        //public static void ExportToExcel(System.Data.DataTable dt, Worksheet excelSheet)
        //{
        //    // Copy the DataTable to an object array
        //    var rawData = new object[dt.Rows.Count + 1, dt.Columns.Count];

        //    // Copy the column names to the first row of the object array
        //    for (var col = 0; col < dt.Columns.Count; col++)
        //    {
        //        rawData[0, col] = dt.Columns[col].ColumnName;
        //    }

        //    // Copy the values to the object array
        //    for (var col = 0; col < dt.Columns.Count; col++)
        //    {
        //        for (var row = 0; row < dt.Rows.Count; row++)
        //        {
        //            rawData[row + 1, col] = dt.Rows[row].ItemArray[col];
        //        }
        //    }


        //    // Fast data export to Excel
        //    var excelRange = string.Format("A1:{0}{1}",
        //        LastColumnLetter(dt.Columns.Count), dt.Rows.Count + 1);

        //    excelSheet.get_Range(excelRange, Type.Missing).Value2 = rawData;

        //}

        public static string LastColumnLetter(int columnCount)
        {
            var finalColLetter = string.Empty;
            var colCharset = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var colCharsetLen = colCharset.Length;

            if (columnCount > colCharsetLen)
            {
                finalColLetter = colCharset.Substring(
                    (columnCount - 1) / colCharsetLen - 1, 1);
            }

            finalColLetter += colCharset.Substring(
                    (columnCount - 1) % colCharsetLen, 1);

            return finalColLetter;
        }

        async void simpleAction_ShowReport_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var currentObject = View.CurrentObject as ReportExecution;
            var selectedReport = currentObject?.ReportDefinition as ReportDefinition;
            if (View.CurrentObject == null || selectedReport == null)
                throw new Exception("CurrentObject is null or is not ReportDefinition");

            var time = DateTime.Now;
            const string date = "yy.M.d_h-mm-ss";
            var reportName = $@"{selectedReport.Name}_{time.ToString(date)}.xlsx";
            var destination = selectedReport.Destination;

            if (string.IsNullOrEmpty(destination) || destination.ToLower() == "default")
                destination = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            var oldCaption = simpleAction_ShowReport.Caption;
            try
            {
                EnableAction(simpleAction_ShowReport, false);
                simpleAction_ShowReport.Caption = "Generating ...";
                //FastExportingMethod.ExportData2Excel(selectedReport, /*sqlConnection*/new UnitOfWork(XpoDefault.DataLayer), destination, reportName);

                //await Task.Run(() =>
                //{
                //    //var sqlConnection = (ObjectSpace as XPObjectSpace)?.Session?.Connection as SqlConnection;
                //    using (var uow = new UnitOfWork())
                //    {
                //        uow.Connect();
                //        //var sqlConnection = uow.Connection as SqlConnection;
                //        //if (sqlConnection == null)
                //        //    throw new InvalidOperationException("sqlConnection is null");

                //        //if (sqlConnection.State != ConnectionState.Open)
                //        //    sqlConnection.Open();

                //        var key = Registry.ClassesRoot;
                //        var excelKey = key.OpenSubKey("Excel.Application");
                //        var excelInstalled = excelKey == null;

                //        if (!excelInstalled)
                //            FastExportingMethod.ExportData2Excel(selectedReport, /*sqlConnection*/uow, destination, reportName);
                //        else
                //            try
                //            {
                //                var form = (View.Control as Control)?.FindForm();
                //                form?.Invoke(new MethodInvoker(() => simpleAction_ShowReport.Caption = "Waiting for server"));
                //                SendReportExecutionRequest(selectedReport, Path.Combine(destination, reportName), uow);
                //            }
                //            catch (Exception ex)
                //            {
                //                XtraMessageBox.Show("მოხდა შეცდომა რეპორტის გენერირებისას, სადეთ თავიდან ან მიმართეთ სისტემის ადმინისტრატორს." + Environment.NewLine + ex);
                //                throw;
                //            }

                //        uow.CommitChanges();

                //        var filePath = (Path.Combine(destination, reportName));
                //        var argument = @"/select, " + filePath;
                //        Process.Start("explorer.exe", argument);
                //        //sqlConnection.Close();
                //    }
                //});
            }
            finally
            {
                EnableAction(simpleAction_ShowReport, true);
                simpleAction_ShowReport.Caption = oldCaption;
                ObjectSpace.CommitChanges();
            }
        }


        public void SendReportExecutionRequest(ReportDefinition selectedReport, string path, UnitOfWork uow)
        {
            var reportServiceDestination = "";// ConfigurationStatic.GetParameterValue("ReportingServerAddress", uow);
            if (string.IsNullOrEmpty(reportServiceDestination))
                throw new InvalidOperationException("Reporting Sevrer Address Not Found");

            var def = new DataExportRequest() { ReportDefinitionID = selectedReport.ID, Parameters = new List<QueryParameterHelper>() };
            foreach (var parameter in selectedReport.QueryParametersCollection)
                def.Parameters.Add(new QueryParameterHelper() { ParameterName = parameter.ParameterName, ParameterValue = parameter.ParameterValue.ToString() });

            var xml = FastExportingMethod.Serialize(def);
            var xmlBytes = Encoding.UTF8.GetBytes(xml);
            var base64Xml = Convert.ToBase64String(xmlBytes);
            var encodedBase64Xml = WebUtility.UrlEncode(base64Xml);
            var request = WebRequest.Create($"http://{reportServiceDestination}/?data={encodedBase64Xml}");
            request.Timeout = 600000;

            var response = request.GetResponse();

            using (var file = response.GetResponseStream())
            using (var fileStream = new FileStream(path, FileMode.Create))
                file?.CopyTo(fileStream);
        }

        public static void EnableAction(ActionBase action, bool enable)
        {
            action.Enabled.Clear();
            action.Enabled.SetItemValue("ForSomeReason", enable);
        }
    }
}