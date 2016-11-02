using DevExpress.Xpo;
using DoSo.Reporting.BusinessObjects.SMS;
using NewBaseModule.BisinessObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static DoSo.Reporting.BusinessObjects.Base.DoSoMessageBase;

namespace DoSo.MessageSendService
{
    public static class SmsSender
    {
        static object _locker = new object();
        public static void SendAll(Timer timer)
        {
            lock (_locker)
                try
                {
                    using (var unitOfWork = new UnitOfWork(XpoDefault.DataLayer))
                    {
                        var allSmsToSend = unitOfWork.Query<DoSoSms>().Where(x => x.Status == MessageStatusEnum.Active && x.ExpiredOn == null && x.SendingDate < DateTime.Now && (x.DoSoSmsSchedule == null || x.DoSoSmsSchedule.IsActive));

                        foreach (var smsToSend in allSmsToSend)
                            Send(smsToSend, unitOfWork);
                    }
                }
                catch (Exception ex)
                {
                    HS.CreateExceptionLog(ex.Message, ex.ToString(), 6);
                }
        }

        public static void Send(DoSoSms smsToSend, UnitOfWork unitOfWork)
        {
            try
            {
                var smsTo = smsToSend.SmsTo.NormalizeTelNumber();

                if (smsToSend.SmsText.Contains("#"))
                {
                    smsToSend.SmsText = smsToSend.SmsText.Replace("#", "N ");
                    smsToSend.StatusComment = "(# Replaced To  N) ;";
                }

                var smsText = smsToSend.SmsText;
                var url = string.Format(HS.SmsBaseUrl, HS.SmsClientID, smsTo, HS.SmsSenderName, smsText);

                //http://smsoffice.ge/api/send.aspx?key={0}&destination={1}&sender={2}&content={3}
                //http://smsoffice.ge/api/send.aspx?key=123456&destination=995577123456&sender=smsoffice&content=TestMessage

                var request = HttpWebRequest.Create(url);
                if (request == null)
                    throw new Exception("SmsSender ->> HttpWebRequest Is Null");

                request.Proxy = null;

                using (var response = request.GetResponse())
                {
                    if (response == null)
                        throw new Exception("SmsSender ->> Response Is Null");

                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        if (reader == null)
                            throw new Exception("SmsSender - Reader Is Null");

                        var urlText = reader.ReadToEnd();

                        if (urlText?.ToLower()?.Trim() == HS.SmsSuccessCode?.ToLower()?.Trim())
                        {
                            smsToSend.Status = MessageStatusEnum.Sent;
                            smsToSend.SentDate = DateTime.Now;
                        }
                        else
                        {
                            smsToSend.CancelMessage(string.Format($"Error Code {urlText}  Date - {DateTime.Now}"), MessageStatusEnum.CancelledByService);
                        }
                    }
                }

                unitOfWork.CommitChanges();
            }
            catch (Exception ex)
            {
                smsToSend.CancelMessage($"Exception Was Thrown. See Exception log ({DateTime.Now})", MessageStatusEnum.CancelledByService);
                HS.CreateExceptionLog(ex.Message, ex.ToString(), 5);
            }

        }
    }
}
