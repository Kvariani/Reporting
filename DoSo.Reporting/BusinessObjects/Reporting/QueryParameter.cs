using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.StateMachine;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using NewBaseModule.BisinessObjects;

namespace DoSo.Reporting.BusinessObjects
{
    //[DefaultClassOptions]
    //[NavigationItem("Reports")]
    [DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    public class QueryParameter : NewXPLiteObjectEx
    {
        public QueryParameter(Session session) : base(session) { }

        private string fParameterName;

        public string ParameterName
        {
            get { return fParameterName; }
            set { SetPropertyValue(nameof(ParameterName), ref fParameterName, value); }
        }

        public string Value
        {
            get
            {
                if (DataType == DataTypeEnnum.BusinessObject)
                    return
                        XafTypesInfo.Instance.FindTypeInfo(BusinessObjectType)?
                            .KeyMember?.GetValue(fParameterValue)?
                            .ToString() ?? string.Empty;
                return fParameterValue?.ToString();
            }
        }

        private object fParameterValue;
        [ModelDefault("DisplayFormat", "yyyy.MM.dd HH:mm")]
        [ModelDefault("EditMask", "G")]
        public object ParameterValue
        {
            get { return fParameterValue; }
            set
            {
                //if (DataType == DataTypeEnnum.Datetime && value is DateTime && !string.IsNullOrEmpty(value?.ToString()))
                //{
                //    var sysFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
                //    value = ((DateTime)value).ToString($"{sysFormat}");
                //}
                SetPropertyValue(nameof(ParameterValue), ref fParameterValue, value);
            }
        }

        private string fDefaultValue;

        public string DefaultValue
        {
            get { return fDefaultValue; }
            set { SetPropertyValue(nameof(DefaultValue), ref fDefaultValue, value); }
        }

        //private ReportDefinition fReportDefinition;

        //[Association("ReportDefinition-QueryParameter")]
        //public ReportDefinition ReportDefinition
        //{
        //    get { return fReportDefinition; }
        //    set { SetPropertyValue(nameof(ReportDefinition), ref fReportDefinition, value); }
        //}


        private bool fAllowBlank;

        [XafDisplayName("Allow Blank")]
        public bool AllowBlank
        {
            get { return fAllowBlank; }
            set { SetPropertyValue(nameof(AllowBlank), ref fAllowBlank, value); }
        }

        private bool fAllowNull;

        [XafDisplayName("Allow Null")]
        public bool AllowNull
        {
            get { return fAllowNull; }
            set { SetPropertyValue(nameof(AllowNull), ref fAllowNull, value); }
        }

        private bool fAllowMultipleValues;

        [XafDisplayName("Allow Multiple Values")]
        public bool AllowMultipleValues
        {
            get { return fAllowMultipleValues; }
            set { SetPropertyValue(nameof(AllowMultipleValues), ref fAllowMultipleValues, value); }
        }

        private DataTypeEnnum fDataType;
        [ImmediatePostData]
        [XafDisplayName("Data Type")]
        public DataTypeEnnum DataType
        {
            get { return fDataType; }
            set { SetPropertyValue(nameof(DataType), ref fDataType, value); }
        }

        [ValueConverter(typeof(TypeToStringConverter))]
        [TypeConverter(typeof(StateMachineTypeConverter))]
        [ImmediatePostData]
        public Type BusinessObjectType
        {
            get { return GetPropertyValue<Type>(nameof(BusinessObjectType)); }
            set { SetPropertyValue(nameof(BusinessObjectType), value); }
        }


        //private EnumTypeHelper fEnumType;
        ////[NonPersistent]
        //[ValueConverter(typeof(TypeToStringConverter))]
        //[DataSourceProperty(nameof(EnumTypes))]
        //public string EnumType
        //{
        //    get { return GetPropertyValue<string>(nameof(EnumType)); }
        //    set { SetPropertyValue(nameof(EnumType), value); }
        //}


        [ValueConverter(typeof(StringObjectToStringConverter))]
        [DataSourceProperty(nameof(EnumTypes))]
        //[LookupEditorModeAttribute(LookupEditorMode.Auto)]
        public StringObject EnumType
        {
            get { return GetPropertyValue<StringObject>(nameof(EnumType)); }
            set { SetPropertyValue(nameof(EnumType), value); }
        }

        //[Browsable(false)]
        //public IEnumerable<StringObject> EnumTypes => new List<StringObject>() { new StringObject("asd") };

        //[DataSourceProperty(nameof(EnumTypes))]
        //public EnumTypeHelper EnumType
        //{
        //    //get; set;
        //    get { return GetPropertyValue<EnumTypeHelper>(nameof(EnumType)); }
        //    set { SetPropertyValue(nameof(EnumType), value); }
        //}

        List<StringObject> EnumTypes
        {
            get
            {
                Func<Assembly, string, bool> assemblyFullNameDoesNotContain = (assembly, name2Exclude) => !assembly.FullName.ToLowerInvariant().Contains(name2Exclude.ToLowerInvariant());
                Func<string, string, bool> stringDoesNotStartWith = (containingText, containedText) => !containingText.ToLowerInvariant().StartsWith(containedText.ToLowerInvariant());

                var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a =>
               assemblyFullNameDoesNotContain(a, "System")
               && assemblyFullNameDoesNotContain(a, "DevExpress.")
               && assemblyFullNameDoesNotContain(a, "Microsoft")
               && assemblyFullNameDoesNotContain(a, "mscorlib")
               && assemblyFullNameDoesNotContain(a, "vshost")
               && assemblyFullNameDoesNotContain(a, "Autofac") && !a.IsDynamic)
               .SelectMany(a => a.GetExportedTypes().Where(et => et.IsEnum
                    && stringDoesNotStartWith(et.FullName, "System")
                    && stringDoesNotStartWith(et.FullName, "Microsoft")
                    && stringDoesNotStartWith(et.FullName, "Accessibility")))
               .Select(a => new StringObject(a.FullName))
               .ToList();

                return assemblies;
            }
        }

        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);
            if (propertyName == nameof(DataType))
            {
                ParameterValue = null;
                if ((DataTypeEnnum)newValue == DataTypeEnnum.BusinessObject && oldValue != newValue)
                    BusinessObjectType = null;
            }
            if (propertyName == nameof(BusinessObjectType))
                ParameterValue = null;
        }

        protected override void OnSaving()
        {
            base.OnSaving();

            if (ParameterValue != null)
                if (DataType == DataTypeEnnum.Datetime)
                {
                    var date = DateTime.Parse(ParameterValue.ToString());
                    DefaultValue = date.ToString("yyyy.MMM.dd HH:mm");
                }
                else
                    DefaultValue = ParameterValue.ToString();
        }
    }


    public enum DataTypeEnnum
    {
        Text = 1,
        Datetime = 2,
        Integer = 3,
        BusinessObject = 4,
        Enum = 5
    }
}
