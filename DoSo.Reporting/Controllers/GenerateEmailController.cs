using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Xpo;
//using DoSoMessageSendService;
using DevExpress.Xpo;
using DoSo.Reporting.BusinessObjects;
using DoSo.Reporting.Generators;
using System;

namespace DoSo.Reporting.Controllers
{

    public partial class GenerateEmailController : ObjectViewController<DetailView, DoSoReportSchedule>
    {
        public GenerateEmailController()
        {
            InitializeComponent();
            RegisterActions(components);
            
        }

        private void simpleAction_GenerateEmails_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var generator = new MailGenerator();
            using (var unitOfWork = new UnitOfWork((ObjectSpace as XPObjectSpace).Session.DataLayer))
            {
                var exception = generator.GenerateEmailFromSchedule(unitOfWork.GetObjectByKey<DoSoReportSchedule>(ViewCurrentObject.ID), unitOfWork);

                if (!string.IsNullOrEmpty(exception))
                    throw new Exception(exception);

                ObjectSpace.Refresh();
            }
        }
    }
}
