using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Xpo;
using DoSo.Reporting.BusinessObjects.Base;

namespace DoSo.Reporting.Controllers
{
    public partial class GenerateMessagesController : ObjectViewController<DetailView, DoSoScheduleBase>
    {
        public GenerateMessagesController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        private void simpleAction_GenerateSms_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            //var generator = new SmsGenerator();
            //using (var unitOfWork = new UnitOfWork((ObjectSpace as XPObjectSpace).Session.DataLayer))
            //{
            //    var exception = generator.GenerateSmsFromSchedule(unitOfWork.GetObjectByKey<DoSoSmsSchedule>(ViewCurrentObject.ID), unitOfWork);
            //    if (!string.IsNullOrEmpty(exception))
            //        throw new Exception(exception);
            //    ObjectSpace.Refresh();
            //}
        }


        private void SimpleAction_GenerateMessages_Execute(object sender, DevExpress.ExpressApp.Actions.SimpleActionExecuteEventArgs e)
        {
            using (var uow = new UnitOfWork(XpoDefault.DataLayer))
            {
                var currentObject = uow.GetObjectByKey<DoSoScheduleBase>(ViewCurrentObject.ID);
                //currentObject.CreateDataSourceFromXml();
                var list = currentObject.GenerateMessages(uow, false);
                currentObject.GetNextExecutionDate();
                uow.CommitChanges();
            }
        }
    }
}
