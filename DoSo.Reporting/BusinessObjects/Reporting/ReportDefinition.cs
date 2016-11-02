using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using DevExpress.ExpressApp.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DoSo.Reporting.BusinessObjects;
using NewBaseModule.BisinessObjects;
using System.Windows.Forms;
using System;
using DevExpress.XtraSpreadsheet;

namespace Reporting.Reporting.BusinessObjects
{
    [DefaultClassOptions]
    [NavigationItem("Reports")]
    public class ReportDefinition : NewXPLiteObjectEx
    {
        public ReportDefinition(Session session) : base(session) { }

        private string fName;
        [SearchMemberOptions(SearchMemberMode.Include)]
        public string Name
        {
            get { return fName; }
            set { SetPropertyValue(nameof(Name), ref fName, value); }
        }

        private string fSQL;
        [Size(SizeAttribute.Unlimited)]
        public string SQL
        {
            get { return fSQL; }
            set { SetPropertyValue(nameof(SQL), ref fSQL, value); }
        }

        private string fTemplateData;
        [Size(SizeAttribute.Unlimited)]
        public string TemplateData
        {
            get { return fTemplateData; }
            set { SetPropertyValue(nameof(TemplateData), ref fTemplateData, value); }
        }

        private FileData fTemplate;
        public FileData Template
        {
            get { return fTemplate; }
            set { SetPropertyValue(nameof(Template), ref fTemplate, value); }
        }

        private string fDescription;
        [Size(SizeAttribute.Unlimited)]
        public string Description
        {
            get { return fDescription; }
            set { SetPropertyValue(nameof(Description), ref fDescription, value); }
        }

        private string fDestination;
        public string Destination
        {
            get { return fDestination; }
            set { SetPropertyValue(nameof(Destination), ref fDestination, value); }
        }




        [Association("ReportDefinition-SqlQuery")]
        //[AllowEditCollection]
        //[AllowNewCollection]
        //[HideLinkUnlinkActions]
        public XPCollection<SqlQuery> SqlQueryCollection => GetCollection<SqlQuery>(nameof(SqlQueryCollection));

        [Association("ReportDefinition-Schedule")]
        //[HideLinkUnlinkActions]
        public XPCollection<DoSoReportSchedule> DoSoEmailSchedulesCollection => GetCollection<DoSoReportSchedule>(nameof(DoSoEmailSchedulesCollection));

        [Association("ReportDefinition-QueryParameter")]
        //[HideLinkUnlinkActions]
        //[AllowEditCollection]
        public XPCollection<QueryParameter> QueryParametersCollection => GetCollection<QueryParameter>(nameof(QueryParametersCollection));

        [Association("ReportDefinition-ReportExecution")]
        //[HideLinkUnlinkActions]
        public XPCollection<ReportExecution> ReportExecutionCollection => GetCollection<ReportExecution>(nameof(ReportExecutionCollection));

        [Association("Group-Definition")]
        public XPCollection<ReportDefinitionGroup> ReportDefinitionGroupsCollection => GetCollection<ReportDefinitionGroup>(nameof(ReportDefinitionGroupsCollection));

        //private ReportDefinitionGroup fReportDefinitionGroup;
        //[Association("ReportDefinitionGroup-ReportDefinition")]
        //public ReportDefinitionGroup ReportDefinitionGroup
        //{
        //    get { return fReportDefinitionGroup; }
        //    set { SetPropertyValue(nameof(ReportDefinitionGroup), ref fReportDefinitionGroup, value); }
        //}

        private bool templateWasChanged;

        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);

            if (propertyName == nameof(Template))
                templateWasChanged = true;
        }

        protected override void OnSaving()
        {
            base.OnSaving();

            if (templateWasChanged || string.IsNullOrEmpty(TemplateData))
                TemplateData = GenerateXmlFromTemplate();
        }

        public string GenerateXmlFromTemplate()
        {
            if (Template == null)
                return "";



            var spreadsheetControl = new SpreadsheetControl();
            spreadsheetControl.CreateNewDocument();


            //existingDatas = new List<ExistingData>();
            var tempName = Path.GetTempFileName() + @".Xlsx";
            //tempPath = Path.Combine(Path.GetTempPath(), tempName);

            //using (var stream = new FileStream(tempPath, FileMode.Create))
            //    Template.SaveToStream(stream);

            //object o = Missing.Value;
            //excelWorkbook = excelApp.Workbooks.Open(tempPath, o, o, o, o, o, o, o, o, o, o, o, o, o, o);
            var dataList = GetExistingData(/*excelWorkbook*/);

            if (dataList.Count == 0)
                return "";

            var xmlSerializer = new XmlSerializer(dataList.GetType());

            using (var writer = new StringWriter())
            using (var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings { Indent = true, NewLineChars = "\r\n" }))
            {
                xmlSerializer.Serialize(xmlWriter, dataList);
                var data = writer.ToString();
                //excelWorkbook.Close(false);
                //File.Delete(tempPath);
                return data;
            }
        }


        List<ExistingData> GetExistingData(/*Workbook xlWorkBook*/)
        {
            //var sheetsCount = xlWorkBook.Worksheets.Count;
            List<ExistingData> existingDatas = new List<ExistingData>();
            string xmlText = "";

            //for (int i = 0; i < sheetsCount; i++)
            //{
            //    var xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.Item[i + 1];

            //    xlWorkSheet.Columns.ClearFormats();
            //    xlWorkSheet.Rows.ClearFormats();

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
            //                existingDatas.Add(new ExistingData() { SheetIndex = i, Row = singleItem.Row, Column = singleItem.Column, Value = value });
            //            }
            //        }
            //    }
            //}

            return existingDatas;
        }
    }
}
