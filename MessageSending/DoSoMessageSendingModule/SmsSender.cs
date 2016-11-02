using DevExpress.Xpo;
using DoSo.Reporting.BusinessObjects.SMS;
using DoSo.Reporting.Generators;
using NewBaseModule.BisinessObjects;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace DoSoMessageSendService
{
    public class SmsSender
    {
        static bool executing;
        static bool ConfigurationWasTaken;

        static int SmsSendingTimeOut;

        static string smsBaseUrl;
        //static string smsClientUsername;
        //static string smsClientPassword;
        static string smsClientID;
        //static string smsServiceID;
        static string smsSenderName;
        static string smsSuccessCode;

        public static void OnCallBack(Timer timer)
        {
            if (timer == null)
                GeneratorHelper.CreateLogFileWithException("SMS Sender Timer Is Null");

            if (executing)
            {
                if (SmsSendingTimeOut > 0)
                    timer.Change(SmsSendingTimeOut, Timeout.Infinite);
                return;
            }

            executing = true;

            try
            {
                using (var unitOfWork = new UnitOfWork(new SimpleDataLayer(GeneratorHelper.GetConnection())))
                {
                    GetConfiguration(unitOfWork);

                    var allSmsToSend = unitOfWork.Query<DoSoSms>().Where(x => !x.IsSent
                                                                        && !x.IsCanceled
                                                                        && x.ExpiredOn == null
                                                                        && x.SendingDate < DateTime.Now);

                    foreach (var smsToSend in allSmsToSend)
                    {
                        var schedule = smsToSend.DoSoSmsSchedule;

                        if (schedule != null)
                            if (!schedule.IsActive)
                                continue;

                        var smsTo = NormalizeNumber(smsToSend.SmsTo);

                        if (smsToSend.SmsText.Contains("#"))
                        {
                            smsToSend.SmsText = smsToSend.SmsText.Replace("#", "N ");
                            smsToSend.StatusComment = "# Replaced To  N";
                        }

                        var smsText = smsToSend.SmsText;
                        var url = string.Format(smsBaseUrl, smsClientID, smsTo, smsSenderName, smsText);

                        //http://smsoffice.ge/api/send.aspx?key={0}&destination={1}&sender={2}&content={3}
                        //http://smsoffice.ge/api/send.aspx?key=123456&destination=995577123456&sender=smsoffice&content=TestMessage

                        var request = HttpWebRequest.Create(url);
                        if (request == null)
                            GeneratorHelper.CreateLogFileWithException("DoSoSmsSendingModule - request Is Null");
                        request.Proxy = null;

                        using (var response = request.GetResponse())
                        {
                            if (response == null)
                                GeneratorHelper.CreateLogFileWithException("DoSoSmsSendingModule - response Is Null");

                            var reader = new StreamReader(response.GetResponseStream());
                            if (reader == null)
                                GeneratorHelper.CreateLogFileWithException("DoSoSmsSendingModule - reader Is Null");


                            var urlText = reader.ReadToEnd();

                            if (urlText.Trim() == smsSuccessCode.Trim())
                            {
                                smsToSend.IsSent = true;
                                smsToSend.SentDate = DateTime.Now;
                            }
                            else
                            {
                                smsToSend.IsCanceled = true;
                                smsToSend.StatusComment = string.Format("Error Code {0}", urlText);
                            }
                        }

                        unitOfWork.CommitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                executing = false;
                GeneratorHelper.CreateLogFileWithException(ex.ToString());
            }

            if (SmsSendingTimeOut > 0)
                timer.Change(SmsSendingTimeOut, Timeout.Infinite);
            executing = false;
        }

        static void GetConfiguration(UnitOfWork unitOfWork)
        {
            if (ConfigurationWasTaken)
                return;
            try
            {
                //var smsClientUsername = ConfigurationStatic.GetParameterValue(@"DoSoSmsClientUsername", unitOfWork);
                //var smsClientPassword = ConfigurationStatic.GetParameterValue(@"DoSoSmsClientPassword", unitOfWork);
                smsClientID = ConfigurationStatic.GetParameterValue(@"DoSoSmsClientID", unitOfWork);
                //var smsServiceID = ConfigurationStatic.GetParameterValue(@"DoSoSmsServiceID", unitOfWork);
                SmsSendingTimeOut = Convert.ToInt32(ConfigurationStatic.GetParameterValue(@"DoSoSmsSendingTimeOut", unitOfWork));
                smsBaseUrl = ConfigurationStatic.GetParameterValue(@"DoSoSmsBaseUrl", unitOfWork);
                smsSenderName = ConfigurationStatic.GetParameterValue(@"DoSoSmsSenderName", unitOfWork);
                smsSuccessCode = ConfigurationStatic.GetParameterValue(@"DoSoSmssmsSuccessCode", unitOfWork);

                ConfigurationWasTaken = true;
            }
            catch (Exception ex)
            {
                GeneratorHelper.CreateLogFileWithException(ex.ToString());
            }
        }

        static string NormalizeNumber(string unNormalizedNumber)
        {
            var number2Return = unNormalizedNumber.Trim();
            number2Return = unNormalizedNumber.Replace(" ", "");

            if (!number2Return.StartsWith("995"))
                number2Return = "995" + number2Return;

            return number2Return;

        }
    }
}
