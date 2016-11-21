using DevExpress.Xpo;
using DoSo.Reporting.BusinessObjects;
using NewBaseModule.BisinessObjects;
using System;
using System.Linq;
using System.Threading;

namespace DoSo.Reporting.Generators
{
    public class MailGenerator
    {
        static object _locker = new object();

        public static void GenerateAll(/*Timer timer*/)
        {
            lock (_locker)
                try
                {
                    HS.GetOrCreateSericeStatus(nameof(MailGenerator));
                    using (var unitOfWork = new UnitOfWork(XpoDefault.DataLayer))
                    {
                        var allSchedule = unitOfWork.Query<DoSoReportSchedule>().Where(x => x.IsActive && x.NextExecutionDate < DateTime.Now && x.ExpiredOn == null).ToList();
                        foreach (var item in allSchedule)
                        {
                            if (!HS.EnableMailGenerator)
                                return;
                            item.GenerateMessages(unitOfWork);
                            item.GetNextExecutionDate();
                            unitOfWork.CommitChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    HS.GetOrCreateSericeStatus(nameof(MailGenerator), true);
                    HS.CreateExceptionLog(ex.Message, ex.ToString(), 6);
                }
        }

        //static int EmailGeneratorTimeOut;
        //static bool executing;

        //public void OnCallBack(Timer timer)
        //{
        //    if (executing)
        //    {
        //        if (EmailGeneratorTimeOut > 0)
        //            timer.Change(EmailGeneratorTimeOut, Timeout.Infinite);
        //        return;
        //    }
        //    executing = true;

        //    using (var unitOfWork = new UnitOfWork(new SimpleDataLayer(GeneratorHelper.GetConnection())))
        //    {
        //        if (EmailGeneratorTimeOut == 0)
        //            EmailGeneratorTimeOut = Convert.ToInt32(ConfigurationStatic.GetParameterValue(@"DoSoEmailGeneratorTimeOut", unitOfWork));
        //        var exception = string.Empty;
        //        try
        //        {
        //            var schedules = unitOfWork.Query<DoSoReportSchedule>().Where(x => x.ExpiredOn == null && x.NextExecutionDate < DateTime.Now && x.IsActive);

        //            foreach (var schedule in schedules)
        //                exception += GenerateEmailFromSchedule(schedule, unitOfWork);
        //        }
        //        catch (Exception exc)
        //        { GeneratorHelper.CreateLogFileWithException(exc.ToString()); }

        //        if (!string.IsNullOrWhiteSpace(exception))
        //            GeneratorHelper.CreateLogFileWithException(exception);

        //        if (EmailGeneratorTimeOut > 0)
        //            timer.Change(EmailGeneratorTimeOut, Timeout.Infinite);
        //        executing = false;
        //    }
        //}

        //public string GenerateEmailFromSchedule(DoSoReportSchedule schedule, UnitOfWork unitOfWork)
        //{
        //    var exception = string.Empty;

        //    var objects = GeneratorHelper.GetMyObjects(schedule.ObjectsCriteria, unitOfWork, /*schedule.BusinessObjectFullName*/"", true, 500);

        //    foreach (var item in objects)
        //    {
        //        try
        //        {
        //            var message = GeneratorHelper.GenerateEmail(unitOfWork, item, schedule.MessageSubject, schedule.MessageTo, schedule.MessageCC, schedule.MessageBody, schedule);

        //            if (schedule.ReportData != null)
        //                message.ReportData = schedule.ReportData;

        //            if (message == null)
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
