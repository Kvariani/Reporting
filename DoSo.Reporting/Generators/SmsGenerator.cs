using DevExpress.Xpo;
using DoSo.Reporting.BusinessObjects.SMS;
using NewBaseModule.BisinessObjects;
using System;
using System.Linq;
using System.Threading;

namespace DoSo.Reporting.Generators
{
    public static class SmsGenerator
    {
        static object _locker = new object();
        public static void GenerateAll(Timer timer)
        {
            lock (_locker)
                try
                {
                    
                    using (var unitOfWork = new UnitOfWork(XpoDefault.DataLayer))
                    {
                        var allSchedule = unitOfWork.Query<DoSoSmsSchedule>().Where(x => x.IsActive && x.NextExecutionDate < DateTime.Now && x.ExpiredOn == null);
                        foreach (var item in allSchedule)
                        {
                            if (!HS.EnableSmsGenerator)
                                return;
                            item.GenerateMessages(unitOfWork);
                            item.GetNextExecutionDate();
                            unitOfWork.CommitChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    HS.CreateExceptionLog(ex.Message, ex.ToString(), 6);
                }
        }

        //public SmsGenerator() { }

        //static int SmsGeneratorTimeOut;
        //static bool executing;

        //public void OnCallBack(Timer timer)
        //{
        //    if (executing)
        //    {
        //        if (SmsGeneratorTimeOut > 0)
        //            timer.Change(SmsGeneratorTimeOut, Timeout.Infinite);
        //        return;
        //    }

        //    executing = true;

        //    using (var unitOfWork = new UnitOfWork(XpoDefault.DataLayer))
        //    {
        //        if (SmsGeneratorTimeOut == 0)
        //            SmsGeneratorTimeOut = Convert.ToInt32(ConfigurationStatic.GetParameterValue(@"DoSoSmsGeneratorTimeOut", unitOfWork));

        //        var exception = string.Empty;

        //        try
        //        {
        //            var schedules = unitOfWork.Query<DoSoSmsSchedule>().Where(x => x.NextExecutionDate < DateTime.Now && x.IsActive && x.ExpiredOn == null);

        //            foreach (var schedule in schedules)
        //                exception += GenerateSmsFromSchedule(schedule, unitOfWork);
        //        }

        //        catch (Exception ex)
        //        { GeneratorHelper.CreateLogFileWithException(ex.ToString()); }

        //        if (!string.IsNullOrWhiteSpace(exception))
        //            GeneratorHelper.CreateLogFileWithException(exception);

        //        if (SmsGeneratorTimeOut > 0)
        //            timer.Change(SmsGeneratorTimeOut, Timeout.Infinite);
        //        executing = false;
        //    }
        //}

        //public string GenerateSmsFromSchedule(DoSoSmsSchedule schedule, UnitOfWork unitOfWork)
        //{
        //    var objects = GeneratorHelper.GetMyObjects(schedule.ObjectsCriteria, unitOfWork, /*schedule.BusinessObjectFullName*/"", false, 10000);

        //    var exception = string.Empty;

        //    foreach (var item in objects)
        //    {
        //        try
        //        {
        //            var sms = GeneratorHelper.GenerateSms(unitOfWork, item, schedule.SmsTo, schedule.SmsText, schedule.AllowUnicodeText, schedule.SkipExecutionDate ?? 0, schedule);
        //            if (sms == null)
        //                continue;

        //            schedule.GetNextExecutionDate();
        //            unitOfWork.CommitChanges();
        //        }
        //        catch (Exception ex)
        //        {
        //            exception += ex + Environment.NewLine;
        //            continue;
        //        }
        //    }
        //    return exception;
        //}
    }
}
