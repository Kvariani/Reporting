using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DoSo.Reporting.BusinessObjects;
using DoSo.Reporting.BusinessObjects.Email;
using FastExcelExportingDemoCs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DoSo.BaseModule.BaseMethods;
using BaseModule.BusinessObjects;

namespace DoSoMessageSendService
{
    public class MailGenerator
    {
        static int EmailGeneratorTimeOut;
        static bool executing;

        public void OnCallBack(Timer timer)
        {
            if (executing)
            {
                if (EmailGeneratorTimeOut.IsNull(0) > 0)
                    timer.Change(EmailGeneratorTimeOut, Timeout.Infinite);
                return;
            }
            executing = true;

            using (UnitOfWork unitOfWork = new UnitOfWork(new SimpleDataLayer(SenderHelper.GetConnection())))
            {
                if (EmailGeneratorTimeOut.IsNull(0) == 0)
                    EmailGeneratorTimeOut = Convert.ToInt32(ConfigurationStatic.GetParameterValue(@"DoSoEmailGeneratorTimeOut", unitOfWork));
                string exception = string.Empty;
                try
                {
                    var schedules = unitOfWork.Query<DoSoReportSchedule>().Where(x => x.ExpiredOn == null && x.NextExecutionDate < DateTime.Now && x.IsActive);

                    foreach (var schedule in schedules)
                        exception += GenerateEmailFromSchedule(schedule, unitOfWork);
                }
                catch (Exception exc)
                { SenderHelper.CreateLogFileWithException(exc.ToString()); }

                if (!string.IsNullOrWhiteSpace(exception))
                    SenderHelper.CreateLogFileWithException(exception);

                if (EmailGeneratorTimeOut.IsNull(0) > 0)
                    timer.Change(EmailGeneratorTimeOut, Timeout.Infinite);
                executing = false;
            }
        }

        public string GenerateEmailFromSchedule(DoSoReportSchedule schedule, UnitOfWork unitOfWork)
        {
            string exception = string.Empty;

            var objects = SenderHelper.GetMyObjects(schedule.ObjectsCriteria, unitOfWork, schedule.BusinessObjectFullName, true, 500);

            foreach (var item in objects)
            {
                try
                {

                    var classInfo = unitOfWork.GetClassInfo(item);
                    var properties = unitOfWork.GetProperties(classInfo);
                    var mailTo = new ExpressionEvaluator(properties, schedule.MessageTo).Evaluate(item).With(x => x.ToString());

                    if (string.IsNullOrEmpty(mailTo))
                        continue;

                    var subject = new ExpressionEvaluator(properties, schedule.MessageSubject).Evaluate(item).ToString();
                    var body = new ExpressionEvaluator(properties, schedule.MessageBody).Evaluate(item).ToString();

                    var key = classInfo.KeyProperty.GetValue(item).ToString();

                    var sameMailFromDb = unitOfWork.Query<DoSoEmail>().Where(x =>
                                                                        x.ExpiredOn == null &&
                                                                        x.EmailTo == mailTo &&
                                                                        x.ObjectKey == key &&
                                                                        x.ObjectTypeName == item.GetType().FullName &&
                                                                        x.ReportSchedule == schedule);
                    var oldNotSentMail = sameMailFromDb.Where(x => !x.IsSent && !x.IsCanceled && x.SendingDate < DateTime.Now);
                    foreach (var oldMail in oldNotSentMail)
                    {
                        oldMail.IsCanceled = true;
                        oldMail.StatusComment = "Created New Email";
                    }

                    //var alredySentMails = sameMailFromDb.FirstOrDefault(x =>
                    //                                        x.IsSent &&
                    //                                        !x.IsCanceled &&
                    //                                        x.SentDate.AddDays(schedule.SkipExecutionDate) > DateTime.Now);

                    var email = new DoSoEmail(unitOfWork)
                    {
                        EmailTo = mailTo,
                        EmailSubject = subject,
                        EmailBody = body,
                        SendingDate = DateTime.Now,
                        ObjectKey = key,
                        ObjectTypeName = item.GetType().FullName,
                        //SourceFilePath = path + fullName,
                        ReportSchedule = schedule
                    };

                    if (schedule.ReportDefinition != null)
                    {
                        var path = System.IO.Path.GetTempPath();
                        var name = DateTime.Now.ToString("MMMddHHmmssfff");
                        var fullName = name + ".xlsx";

                        SetParameteValues(schedule, item, unitOfWork);
                        var exported = FastExportingMethod.ExportData2Excel(schedule.ReportDefinition, unitOfWork.Connection as SqlConnection, path, name);
                        if (exported)
                            email.SourceFilePath = path + fullName;
                        else
                            email.EmailBody += string.Format("{0}{0}{1}", Environment.NewLine, schedule.AlternativeText);
                    }

                    schedule.GetNextExecutionDate();
                    unitOfWork.CommitChanges();
                }
                catch (Exception ex)
                {
                    exception += ex + Environment.NewLine;
                    continue;
                }
            }
            return exception;
        }

        static void SetParameteValues(DoSoReportSchedule schedule, object item, UnitOfWork unitOfWork)
        {
            foreach (var parameter in schedule.QueryParametersCollection.Where(x => x.ExpiredOn == null))
            {
                var parameterFromReport = schedule.ReportDefinition.QueryParametersCollection.Where(x => x.ExpiredOn == null && x.ParameterName == parameter.ParameterName);
                var parameterValue = new ExpressionEvaluator(unitOfWork.GetProperties(unitOfWork.GetClassInfo(item)), parameter.ParameterValueExression).Evaluate(item);
                parameterFromReport.FirstOrDefault().ParameterValue = parameterValue.With(x => x.ToString());
            }
        }
    }
}
