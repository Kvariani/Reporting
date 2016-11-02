using DevExpress.ExpressApp.Core;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;

namespace DoSo.Reporting.BusinessObjects
{
    //[DefaultClassOptions]
    //[NavigationItem("Reports")]
    public class ScheduleQueryParameter : QueryParameter
    {
        public ScheduleQueryParameter(Session session) : base(session) { }

        private DoSoReportSchedule fDoSoReportSchedule;
        [Association("DoSoEmailSchedule-QueryParameter")]
        public DoSoReportSchedule DoSoReportSchedule
        {
            get { return fDoSoReportSchedule; }
            set { SetPropertyValue(nameof(DoSoReportSchedule), ref fDoSoReportSchedule, value); }
        }

        private string fParameterValueExression;
        [ElementTypeProperty("DoSoEmailSchedule.TargetObjectType")]
        public string ParameterValueExression
        {
            get { return fParameterValueExression; }
            set { SetPropertyValue(nameof(ParameterValueExression), ref fParameterValueExression, value); }
        }
    }
}
