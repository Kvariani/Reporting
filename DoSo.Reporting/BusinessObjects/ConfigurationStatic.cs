using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;

namespace NewBaseModule.BisinessObjects
{
    //[DefaultClassOptions]
    public class ConfigurationStatic : NewXPLiteObjectEx
    {
        public ConfigurationStatic(Session session) : base(session) { }

        string fParameterName;
        [RuleUniqueValue]
        public string ParameterName
        {
            get { return fParameterName; }
            set { SetPropertyValue(nameof(ParameterName), ref fParameterName, value); }
        }

        string fDescription;
        public string Description
        {
            get { return fDescription; }
            set { SetPropertyValue(nameof(Description), ref fDescription, value); }
        }


        string fParameterValue;
        public string ParameterValue
        {
            get { return fParameterValue; }
            set { SetPropertyValue(nameof(ParameterValue), ref fParameterValue, value); }
        }


        string fGroupName;
        public string GroupName
        {
            get { return fGroupName; }
            set { SetPropertyValue(nameof(GroupName), ref fGroupName, value); }
        }

        ParameterTypeEnum fParameterType;
        public ParameterTypeEnum ParameterType
        {
            get { return fParameterType; }
            set { SetPropertyValue(nameof(ParameterType), ref fParameterType, value); }
        }

        protected override void OnSaving()
        {
            base.OnSaving();

            if (string.IsNullOrEmpty(GroupName))
                GroupName = "Default";
        }

        public enum ParameterTypeEnum
        {
            String = 0,
            Int = 1,
            Decimal = 2,
            Bool = 3
        }
    }
}
