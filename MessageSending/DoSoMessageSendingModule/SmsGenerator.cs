using DevExpress.Xpo;
using DoSo.Reporting.BusinessObjects.SMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoSo.BaseModule.BaseMethods;
using DevExpress.Data.Filtering.Helpers;
using System.Text.RegularExpressions;
using BaseModule.BusinessObjects;
using System.Threading;
using DevExpress.Data.Filtering;

namespace DoSoMessageSendService
{
    public class SmsGenerator
    {
        public SmsGenerator() { }

        static int SmsGeneratorTimeOut;
        static bool executing;

        public void OnCallBack(Timer timer)
        {
            if (executing)
            {
                if (SmsGeneratorTimeOut.IsNull(0) > 0)
                    timer.Change(SmsGeneratorTimeOut, Timeout.Infinite);
                return;
            }

            executing = true;

            using (UnitOfWork unitOfWork = new UnitOfWork(new SimpleDataLayer(SenderHelper.GetConnection())))
            {
                if (SmsGeneratorTimeOut.IsNull(0) == 0)
                    SmsGeneratorTimeOut = Convert.ToInt32(ConfigurationStatic.GetParameterValue(@"DoSoSmsGeneratorTimeOut", unitOfWork));

                string exception = string.Empty;

                try
                {
                    var schedules = unitOfWork.Query<DoSoSmsSchedule>().Where(x => x.NextExecutionDate < DateTime.Now && x.IsActive && x.ExpiredOn == null);

                    foreach (var schedule in schedules)
                        exception += GenerateSmsFromSchedule(schedule, unitOfWork);
                }

                catch (Exception ex)
                { SenderHelper.CreateLogFileWithException(ex.ToString()); }

                if (!string.IsNullOrWhiteSpace(exception))
                    SenderHelper.CreateLogFileWithException(exception);

                if (SmsGeneratorTimeOut.IsNull(0) > 0)
                    timer.Change(SmsGeneratorTimeOut, Timeout.Infinite);
                executing = false;
            }
        }

        public string GenerateSmsFromSchedule(DoSoSmsSchedule schedule, UnitOfWork unitOfWork)
        {
            var objects = SenderHelper.GetMyObjects(schedule.ObjectsCriteria, unitOfWork, schedule.BusinessObjectFullName, false, 10000);

            string exception = string.Empty;

            foreach (var item in objects)
            {
                try
                {
                    var classInfo = unitOfWork.GetClassInfo(item);
                    var properties = unitOfWork.GetProperties(classInfo);

                    var smsTo = new ExpressionEvaluator(properties, schedule.SmsTo).Evaluate(item).With(x => x.ToString());
                    if (string.IsNullOrEmpty(smsTo))
                        continue;

                    var smsText = new ExpressionEvaluator(properties, schedule.SmsText).Evaluate(item).With(x => x.ToString());
                    if (!schedule.AllowUnicodeText)
                        smsText = SenderHelper.ChangeGeorgianText(smsText);

                    var key = classInfo.With(x => x.KeyProperty).With(x => x.GetValue(item)).With(x => x.ToString());
                    var objectTypeName = item.With(x => x.GetType()).With(x => x.FullName);

                    var sameSmsFromDb = unitOfWork.Query<DoSoSms>().Where(x =>
                                                                        x.SmsTo == smsTo &&
                                                                        x.ExpiredOn == null &&
                                                                        x.ObjectKey == key &&
                                                                        x.ObjectTypeName == objectTypeName &&
                                                                        x.DoSoSmsSchedule == schedule);

                    var oldNotSentSms = sameSmsFromDb.Where(x => !x.IsSent && !x.IsCanceled && x.SendingDate < DateTime.Now);

                    foreach (var oldSms in oldNotSentSms)
                    {
                        oldSms.IsCanceled = true;
                        oldSms.StatusComment = "შეიქმნა ახალი SMS";
                    }

                    var alredySentSms = sameSmsFromDb.FirstOrDefault(x =>
                                                                        x.IsSent &&
                                                                        !x.IsCanceled &&
                                                                        x.SentDate.AddDays(schedule.SkipExecutionDate) > DateTime.Now);

                    if (alredySentSms != null)
                        continue;

                    var sms = new DoSoSms(unitOfWork)
                    {
                        SmsTo = smsTo.With(x => x.Trim()).With(x => x.Replace(" ", "")).Replace("-", "").Replace("_", ""),
                        SmsText = smsText,
                        ObjectKey = key,
                        ObjectTypeName = objectTypeName,
                        DoSoSmsSchedule = schedule,
                        SendingDate = DateTime.Now
                    };

                    if (!Regex.IsMatch(sms.SmsTo, @"\d"))
                    {
                        sms.IsCanceled = true;
                        sms.StatusComment = "ტელეფონის ნომერი არასწორ ფორმატშია";
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
    }
}
