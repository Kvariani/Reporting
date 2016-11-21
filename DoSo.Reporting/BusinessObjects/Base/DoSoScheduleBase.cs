using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NewBaseModule.BisinessObjects;
using DevExpress.DataAccess;
using DevExpress.Xpo.Metadata;
using DevExpress.DataAccess.Excel;
using System.Xml;
using System.Xml.Linq;
using DevExpress.ExpressApp.Core;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Native.Sql;
using System.Collections;
using DevExpress.DataAccess.Native.Excel;

namespace DoSo.Reporting.BusinessObjects.Base
{
    //[DefaultClassOptions]
    //[NavigationItem("Reports")]
    //[ImageName("BO_Contact")]
    [DefaultProperty("ScheduleDescription")]
    public class DoSoScheduleBase : NewXPLiteObjectEx
    {
        public DoSoScheduleBase(Session session) : base(session) { }


        #region Days
        private bool fMonday;
        public bool Monday
        {
            get { return fMonday; }
            set { SetPropertyValue(nameof(Monday), ref fMonday, value); }
        }

        private bool fTuesday;
        public bool Tuesday
        {
            get { return fTuesday; }
            set { SetPropertyValue(nameof(Tuesday), ref fTuesday, value); }
        }

        private bool fWednesday;
        public bool Wednesday
        {
            get { return fWednesday; }
            set { SetPropertyValue(nameof(Wednesday), ref fWednesday, value); }
        }

        private bool fThursday;
        public bool Thursday
        {
            get { return fThursday; }
            set { SetPropertyValue(nameof(Thursday), ref fThursday, value); }
        }

        private bool fFriday;
        public bool Friday
        {
            get { return fFriday; }
            set { SetPropertyValue(nameof(Friday), ref fFriday, value); }
        }

        private bool fSaturday;
        public bool Saturday
        {
            get { return fSaturday; }
            set { SetPropertyValue(nameof(Saturday), ref fSaturday, value); }
        }

        private bool fSunday;
        public bool Sunday
        {
            get { return fSunday; }
            set { SetPropertyValue(nameof(Sunday), ref fSunday, value); }
        }
        #endregion

        [NonPersistent]
        public DataComponentBase DataSource { get; set; }
        public ExcelDataSource ExcelDataSource => DataSource as ExcelDataSource;
        public SqlDataSource SqlDataSource => DataSource as SqlDataSource;

        private string fScheduleDescription;
        [Size(200)]
        public string ScheduleDescription
        {
            get { return fScheduleDescription; }
            set { SetPropertyValue(nameof(ScheduleDescription), ref fScheduleDescription, value); }
        }

        private int? fSkipExecutionDate;
        public int? SkipExecutionDate
        {
            get { return fSkipExecutionDate; }
            set { SetPropertyValue(nameof(SkipExecutionDate), ref fSkipExecutionDate, value); }
        }


        private string fSendingDateExpression;
        [Size(SizeAttribute.Unlimited)]
        [ElementTypeProperty("TargetObjectType")]
        [ModelDefault("PropertyEditorType", "DoSo.Reporting.Controllers.PopupExpressionPropertyEditorEx")]
        public string SendingDateExpression
        {
            get { return fSendingDateExpression; }
            set { SetPropertyValue(nameof(SendingDateExpression), ref fSendingDateExpression, value); }
        }


        private string fObjectTypeExpression;
        [Size(SizeAttribute.Unlimited)]
        [ElementTypeProperty("TargetObjectType")]
        [ModelDefault("PropertyEditorType", "DoSo.Reporting.Controllers.PopupExpressionPropertyEditorEx")]
        public string ObjectTypeExpression
        {
            get { return fObjectTypeExpression; }
            set { SetPropertyValue(nameof(ObjectTypeExpression), ref fObjectTypeExpression, value); }
        }

        private string fObjectKeyExpression;
        [Size(SizeAttribute.Unlimited)]
        [ElementTypeProperty("TargetObjectType")]
        [ModelDefault("PropertyEditorType", "DoSo.Reporting.Controllers.PopupExpressionPropertyEditorEx")]
        public string ObjectKeyExpression
        {
            get { return fObjectKeyExpression; }
            set { SetPropertyValue(nameof(ObjectKeyExpression), ref fObjectKeyExpression, value); }
        }

        private DateTime fNextExecutionDate;
        [ModelDefault("DisplayFormat", "{0:dd.MM.yyyy HH:mm}")]
        [ModelDefault("EditMask", "G")]
        [ModelDefault("ModelDefault", "false")]
        public DateTime NextExecutionDate
        {
            get { return fNextExecutionDate; }
            set { SetPropertyValue(nameof(NextExecutionDate), ref fNextExecutionDate, value); }
        }

        private string fExecutionLog;
        [Size(SizeAttribute.Unlimited)]
        public string ExecutionLog
        {
            get { return fExecutionLog; }
            set { SetPropertyValue(nameof(ExecutionLog), ref fExecutionLog, value); }
        }

        private string fExecutionTime;
        [ModelDefault("EditMask", @"([01]?[0-9]|2[0-3]):[0-5][0-9]")]
        [ModelDefault("EditMaskType", "RegEx")]
        [RuleRequiredField(TargetCriteria = "RunEvery_Minute is null || RunEvery_Minute == 0")]
        public string ExecutionTime
        {
            get { return fExecutionTime; }
            set { SetPropertyValue(nameof(ExecutionTime), ref fExecutionTime, value); }
        }

        private int? fRunEvery_Minute;
        [XafDisplayName("Run Every (Minute)")]
        public int? RunEvery_Minute
        {
            get { return fRunEvery_Minute; }
            set { SetPropertyValue(nameof(RunEvery_Minute), ref fRunEvery_Minute, value); }
        }

        private int? fExecutionDate;
        public int? ExecutionDate
        {
            get { return fExecutionDate; }
            set { SetPropertyValue(nameof(ExecutionDate), ref fExecutionDate, value); }
        }

        private int? fExecutionMonth;
        public int? ExecutionMonth
        {
            get { return fExecutionMonth; }
            set { SetPropertyValue(nameof(ExecutionMonth), ref fExecutionMonth, value); }
        }


        //private string fBusinessObjectFullName;
        ////[Browsable(false)]
        //public string BusinessObjectFullName
        //{
        //    get { return fBusinessObjectFullName; }
        //    set { SetPropertyValue(nameof(BusinessObjectFullName), ref fBusinessObjectFullName, value); }
        //}

        private bool fIsActive;
        public bool IsActive
        {
            get { return fIsActive; }
            set { SetPropertyValue(nameof(IsActive), ref fIsActive, value); }
        }

        //public Type BusinessObject
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(BusinessObjectFullName))
        //            return null;
        //        var BusinessObjectInfo = XafTypesInfo.Instance.FindTypeInfo(BusinessObjectFullName);
        //        return BusinessObjectInfo == null ? null : XafTypesInfo.Instance.FindTypeInfo(BusinessObjectFullName).Type;
        //    }
        //    set { BusinessObjectFullName = value.FullName; }
        //}

        private Type TargetObjectType => typeof(XPLiteObject);

        private string fObjectsCriteria;
        [CriteriaOptions(nameof(TargetObjectType))]
        //[ModelDefault("PropertyEditorType", "DoSo.Reporting.Editors.CriteriaPropertyEditorEx")]
        [Size(SizeAttribute.Unlimited)]
        public string ObjectsCriteria
        {
            get { return fObjectsCriteria; }
            set { SetPropertyValue(nameof(ObjectsCriteria), ref fObjectsCriteria, value); }
        }


        private string fDataSourceXml;
        [Size(SizeAttribute.Unlimited)]
        [ImmediatePostData]
        public string DataSourceXml
        {
            get { return fDataSourceXml; }
            set { SetPropertyValue(nameof(DataSourceXml), ref fDataSourceXml, value); }
        }


        public virtual List<DoSoMessageBase> GenerateMessages(Session session, bool prevewOnly = false)
        {
            CreateDataSourceFromXml();
            return null;
        }


        public void CreateDataSourceFromXml()
        {
            try
            {
                XElement root = XElement.Parse(DataSourceXml);
                if (root?.Name?.ToString().ToLower()?.Contains("excel") ?? false)
                {
                    var newDataSource = new ExcelDataSource();
                    newDataSource.LoadFromXml(root);
                    DataSource = newDataSource;
                }
                if (root?.FirstNode?.ToString()?.ToLower().Contains("sql") ?? false)
                {
                    var newDataSource = new SqlDataSource();
                    newDataSource.LoadFromXml(root);
                    DataSource = newDataSource;
                }
            }
            catch (Exception)
            {
                /// rame logi ro movifiqreb
                DataSource = null;


            }
        }

        public List<XPDataObject> GetObjectsFromDataSource()
        {
            if (DataSource == null)
                throw new InvalidOperationException("DataSource Is null");

            DataSource.Fill(null);
            XPDictionary xpDictionary = DevExpress.ExpressApp.Xpo.XpoTypesInfoHelper.GetXpoTypeInfoSource().XPDictionary;
            var newClass = xpDictionary.CreateClass($"NewClass{ID}", new NonPersistentAttribute());

            if (ExcelDataSource != null)
                foreach (var item in ExcelDataSource.Schema)
                    newClass.CreateMember(item.Name, Type.GetType(item.Type.ToString()));
            if (SqlDataSource != null)
                foreach (var item in SqlDataSource.Result.FirstOrDefault().Columns)
                    newClass.CreateMember(item.Name, item.Type);

            XafTypesInfo.Instance.RefreshInfo(newClass.ClassType);

            return ExcelDataSource != null ? GetListFromExcelDataSource(newClass) : GetListFromSqlDatasource(newClass);
        }


        public List<XPDataObject> GetListFromExcelDataSource(XPClassInfo newClass)
        {
            var list = new List<XPDataObject>();
            var resultView = HS.GetResultView(ExcelDataSource);
            foreach (DevExpress.DataAccess.Native.Excel.ViewRow row in resultView)
            {
                var newObject = newClass.CreateObject(Session) as XPDataObject;
                for (int i = 0; i < resultView.Columns.Count; i++)
                    newObject.SetMemberValue(resultView.Columns[i].Name, resultView.Columns[i].GetValue(row));
                list.Add(newObject);
            }

            return list;
        }

        public List<XPDataObject> GetListFromSqlDatasource(XPClassInfo newClass)
        {
            ResultSet rs = (SqlDataSource as IListSource).GetList() as ResultSet;
            ResultTable rt = rs.Tables.First();

            var list = new List<XPDataObject>();

            if (rt != null)
            {

                for (int i = 0; i < rt.Count; i++)
                {
                    var newObject = newClass.CreateObject(Session) as XPDataObject;
                    foreach (ResultColumn rc in rt.Columns)
                        newObject.SetMemberValue(rc.Name, rc.GetValue((ResultRow)((IList)rt)[i]));
                    list.Add(newObject);
                }
            }
            return list;
        }

        public void GetNextExecutionDate()
        {
            //var today = DateTime.Today;
            var days2Add = new List<int>();

            var now = DateTime.Now;

            var year = now.Year;
            var day = now.Day;
            var month = now.Month;
            var hour = now.Hour;
            var minute = now.Minute;

            if (ExecutionMonth != null)
            {
                month = Convert.ToInt32(ExecutionMonth);
                if (month < DateTime.Now.Month)
                    year = year + 1;
            }

            if ((RunEvery_Minute ?? 0) > 0)
            {
                var date = DateTime.Now.AddMinutes(Convert.ToInt32(RunEvery_Minute ?? 0));
                hour = date.Hour;
                minute = date.Minute;
            }
            else
            {
                if (!string.IsNullOrEmpty(ExecutionTime))
                {
                    var time = ExecutionTime.Split(':');
                    hour = Convert.ToInt32(time[0]);
                    minute = Convert.ToInt32(time[1]);
                }
            }

            if (ExecutionDate != null)
            {
                day = Convert.ToInt32(ExecutionDate);
                var tempDate = new DateTime(year, month, day, hour, minute, 0);
                if (tempDate < DateTime.Now)
                    month = month + 1;
            }
            else
            {
                var tempDate = new DateTime(year, month, day, hour, minute, 0);
                if (Monday)
                    AddDayByWeekDays(DayOfWeek.Monday, days2Add, tempDate);
                //days2Add.Add(((int)DayOfWeek.Monday - (int)tempDate.DayOfWeek + 7) % 7);

                if (Tuesday)
                    AddDayByWeekDays(DayOfWeek.Tuesday, days2Add, tempDate);
                //days2Add.Add(((int)DayOfWeek.Tuesday - (int)tempDate.DayOfWeek + 7) % 7);

                if (Wednesday)
                    AddDayByWeekDays(DayOfWeek.Wednesday, days2Add, tempDate);
                //days2Add.Add(((int)DayOfWeek.Wednesday - (int)tempDate.DayOfWeek + 7) % 7);

                if (Thursday)
                    AddDayByWeekDays(DayOfWeek.Thursday, days2Add, tempDate);
                //days2Add.Add(((int)DayOfWeek.Thursday - (int)tempDate.DayOfWeek + 7) % 7);

                if (Friday)
                    AddDayByWeekDays(DayOfWeek.Friday, days2Add, tempDate);
                //days2Add.Add(((int)DayOfWeek.Friday - (int)tempDate.DayOfWeek + 7) % 7);

                if (Saturday)
                    AddDayByWeekDays(DayOfWeek.Saturday, days2Add, tempDate);
                //days2Add.Add(((int)DayOfWeek.Saturday - (int)tempDate.DayOfWeek + 7) % 7);

                if (Sunday)
                    AddDayByWeekDays(DayOfWeek.Sunday, days2Add, tempDate);
                //days2Add.Add(((int)DayOfWeek.Sunday - (int)tempDate.DayOfWeek + 7) % 7);

                if (days2Add.Count == 0)
                    throw new Exception("გაგზავნის თარიღის დადგენა ვერ მოხერხდა.");

                day = DateTime.Now.AddDays(days2Add.Min(x => x)).Day;
                if (day < DateTime.Now.Day)
                    month = month + 1;
            }
            var datetime = new DateTime(year, month, day, hour, minute, 0);
            if (datetime > DateTime.Now)
                NextExecutionDate = datetime;
            else
                NextExecutionDate = datetime.AddDays(1);
        }

        public void AddDayByWeekDays(DayOfWeek day, List<int> days2Add, DateTime tempDate)
        {
            var tempDayOfWeek = (int)tempDate.DayOfWeek;
            var dayOfWeek = (int)day;
            if (tempDayOfWeek == dayOfWeek && tempDate < DateTime.Now)
                days2Add.Add(7);
            else
                days2Add.Add((dayOfWeek - tempDayOfWeek + 7) % 7);
        }

        protected override void OnSaving()
        {
            if (ExecutionDate == 0 || ExecutionDate > 31)
                throw new Exception("არარეალური თარიღი");

            if (ExecutionMonth == 0 || ExecutionMonth > 12)
                throw new Exception("არარეალური თვე");

            base.OnSaving();

            GetNextExecutionDate();
        }
    }
}
