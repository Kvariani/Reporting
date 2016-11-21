using DevExpress.DataAccess.Excel;
using DevExpress.DataAccess.Native.Excel;
using DevExpress.Xpo;
using DoSo.Reporting.BusinessObjects;
using DoSo.Reporting.BusinessObjects.Base;
using NewBaseModule.BisinessObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static NewBaseModule.BisinessObjects.ConfigurationStatic;

public static class HS
{
    public static bool CheckThreadState(this Thread thread)
    {
        if (thread.ThreadState == System.Threading.ThreadState.Running || thread.ThreadState == System.Threading.ThreadState.WaitSleepJoin)
            return true;
        return false;
    }

    static object _locker = new object();
    public static void GetOrCreateSericeStatus(string name, bool setExeptionTime = false)
    {
        lock (_locker)
            using (var uow = new UnitOfWork(XpoDefault.DataLayer))
            {
                var data = uow.Query<DoSoServiceStatus>().FirstOrDefault(x => x.ServiceName == name);
                if (data == null)
                    data = new DoSoServiceStatus(uow) { ServiceName = name };
                if (setExeptionTime)
                    data.LastExceptionTime = DateTime.Now;
                else
                    data.LastActiveTime = DateTime.Now;
                uow.CommitChanges();
            }
    }

    public static DataView GetResultView(ExcelDataSource dataSource)
    {
        var list = (dataSource as IListSource).GetList();
        if (list is DataView)
            return list as DataView;
        var dt = (list as System.Data.DataViewManager)?.DataSet?.Tables[0];
        DataView resultView = ((IListSource)(dataSource)).GetList() as DevExpress.DataAccess.Native.Excel.DataView;
        return resultView;
    }


    public static void CreateExceptionLog(string message, string stackTrace, int level)
    {
        string newMessage = "";
        try
        {
            using (var uow = new UnitOfWork(XpoDefault.DataLayer))
            {
                new DoSoExceptionLog(uow) { Message = message, StackTrace = stackTrace, Level = level, CreatedOn = DateTime.Now };
                uow.CommitChanges();
                return;
            }

        }
        catch (Exception ex) { newMessage = ex.ToString(); }

        try
        {
            using (TextWriter tw = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ErrrorLog_" + DateTime.Now.ToString("ddHHmmssfff") + ".txt"), true))
            {
                tw.WriteLine("NewMessage  ---------   " + newMessage);
                tw.WriteLine($"Level - {level} --->>> MainMessage  --------->>> {message}");
                tw.WriteLine("StackTrace  ---------   " + stackTrace);
                tw.Close();
            }
        }
        catch (Exception) {/*Ignored*/}
    }

    public static void GetConfiguration()
    {
        try
        {
            using (var uow = new UnitOfWork(XpoDefault.DataLayer))
            {
                var allConfigFromDb = uow.Query<ConfigurationStatic>().ToList();

                EnableSmsGenerator = Convert.ToBoolean(allConfigFromDb.FirstOrDefault(x => x.ParameterName == nameof(EnableSmsGenerator))?.ParameterValue);
                EnableMailGenerator = Convert.ToBoolean(allConfigFromDb.FirstOrDefault(x => x.ParameterName == nameof(EnableMailGenerator))?.ParameterValue);
                EnableSmsSender = Convert.ToBoolean(allConfigFromDb.FirstOrDefault(x => x.ParameterName == nameof(EnableSmsSender))?.ParameterValue);
                EnableMailSender = Convert.ToBoolean(allConfigFromDb.FirstOrDefault(x => x.ParameterName == nameof(EnableMailSender))?.ParameterValue);

                GeteratedFilesName = allConfigFromDb.FirstOrDefault(x => x.ParameterName == nameof(GeteratedFilesName))?.ParameterValue;
                SmsBaseUrl = allConfigFromDb.FirstOrDefault(x => x.ParameterName == nameof(SmsBaseUrl))?.ParameterValue;
                SmsClientUsername = allConfigFromDb.FirstOrDefault(x => x.ParameterName == nameof(SmsClientUsername))?.ParameterValue;
                SmsClientPassword = allConfigFromDb.FirstOrDefault(x => x.ParameterName == nameof(SmsClientPassword))?.ParameterValue;
                SmsClientID = allConfigFromDb.FirstOrDefault(x => x.ParameterName == nameof(SmsClientID))?.ParameterValue;
                SmsServiceID = allConfigFromDb.FirstOrDefault(x => x.ParameterName == nameof(SmsServiceID))?.ParameterValue;
                SmsSenderName = allConfigFromDb.FirstOrDefault(x => x.ParameterName == nameof(SmsSenderName))?.ParameterValue;
                SmsSuccessCode = allConfigFromDb.FirstOrDefault(x => x.ParameterName == nameof(SmsSuccessCode))?.ParameterValue;

                SmtpServer = allConfigFromDb.FirstOrDefault(x => x.ParameterName == nameof(SmtpServer))?.ParameterValue;
                SmtpUserName = allConfigFromDb.FirstOrDefault(x => x.ParameterName == nameof(SmtpUserName))?.ParameterValue;
                SmtpPassword = allConfigFromDb.FirstOrDefault(x => x.ParameterName == nameof(SmtpPassword))?.ParameterValue;
                EMailFrom = allConfigFromDb.FirstOrDefault(x => x.ParameterName == nameof(EMailFrom))?.ParameterValue;
                EnableSsl = Convert.ToBoolean(allConfigFromDb.FirstOrDefault(x => x.ParameterName == nameof(EnableSsl))?.ParameterValue);
                UseDefaultCredentials = Convert.ToBoolean(allConfigFromDb.FirstOrDefault(x => x.ParameterName == nameof(UseDefaultCredentials))?.ParameterValue);
                SmtpPort = Convert.ToInt32(allConfigFromDb.FirstOrDefault(x => x.ParameterName == nameof(SmtpPort))?.ParameterValue);

            }
        }
        catch (Exception ex)
        {
            CreateExceptionLog(ex.Message, ex.ToString(), 10);
        }
    }

    public static string NormalizeTelNumber(this string unNormalizedNumber)
    {
        var number2Return = unNormalizedNumber.Trim();
        number2Return = unNormalizedNumber.Replace(" ", "");

        if (!number2Return.StartsWith("995"))
            number2Return = "995" + number2Return;

        return number2Return;

    }

    public static void InitializeConfigItems()
    {
        CreateConfigurationItems();
        GetConfiguration();
    }

    public static void CreateConfigurationItems()
    {
        try
        {
            using (var uow = new UnitOfWork(XpoDefault.DataLayer))
            {
                var allConfigFromDb = uow.Query<ConfigurationStatic>().ToList();


                uow.CreateConfigItemIfNotExists(allConfigFromDb, nameof(EnableSmsGenerator), ParameterTypeEnum.Bool, "Main");
                uow.CreateConfigItemIfNotExists(allConfigFromDb, nameof(EnableMailGenerator), ParameterTypeEnum.Bool, "Main");
                uow.CreateConfigItemIfNotExists(allConfigFromDb, nameof(EnableSmsSender), ParameterTypeEnum.Bool, "Main");
                uow.CreateConfigItemIfNotExists(allConfigFromDb, nameof(EnableMailSender), ParameterTypeEnum.Bool, "Main");
                uow.CreateConfigItemIfNotExists(allConfigFromDb, nameof(GeteratedFilesName), ParameterTypeEnum.String, "Main");
                uow.CreateConfigItemIfNotExists(allConfigFromDb, nameof(SmsBaseUrl), ParameterTypeEnum.String, "SMS");
                uow.CreateConfigItemIfNotExists(allConfigFromDb, nameof(SmsClientUsername), ParameterTypeEnum.String, "SMS");
                uow.CreateConfigItemIfNotExists(allConfigFromDb, nameof(SmsClientPassword), ParameterTypeEnum.String, "SMS");
                uow.CreateConfigItemIfNotExists(allConfigFromDb, nameof(SmsClientID), ParameterTypeEnum.String, "SMS");
                uow.CreateConfigItemIfNotExists(allConfigFromDb, nameof(SmsServiceID), ParameterTypeEnum.String, "SMS");
                uow.CreateConfigItemIfNotExists(allConfigFromDb, nameof(SmsSenderName), ParameterTypeEnum.String, "SMS");
                uow.CreateConfigItemIfNotExists(allConfigFromDb, nameof(SmsSuccessCode), ParameterTypeEnum.String, "SMS");

                uow.CreateConfigItemIfNotExists(allConfigFromDb, nameof(SmtpServer), ParameterTypeEnum.String, "EMail");
                uow.CreateConfigItemIfNotExists(allConfigFromDb, nameof(SmtpUserName), ParameterTypeEnum.String, "EMail");
                uow.CreateConfigItemIfNotExists(allConfigFromDb, nameof(SmtpPassword), ParameterTypeEnum.String, "EMail");
                uow.CreateConfigItemIfNotExists(allConfigFromDb, nameof(EMailFrom), ParameterTypeEnum.String, "EMail");
                uow.CreateConfigItemIfNotExists(allConfigFromDb, nameof(EnableSsl), ParameterTypeEnum.Bool, "EMail");
                uow.CreateConfigItemIfNotExists(allConfigFromDb, nameof(UseDefaultCredentials), ParameterTypeEnum.Bool, "EMail");
                uow.CreateConfigItemIfNotExists(allConfigFromDb, nameof(SmtpPort), ParameterTypeEnum.Int, "EMail");
            }
        }
        catch (Exception ex)
        {

        }

    }

    public static void CreateConfigItemIfNotExists(this UnitOfWork uow, List<ConfigurationStatic> existingItems, string configName, ParameterTypeEnum type, string groupName)
    {
        var existingItem = existingItems.FirstOrDefault(x => x.ParameterName == configName);
        if (existingItem == null)
        {
            new ConfigurationStatic(uow) { ParameterName = configName, ParameterType = type, GroupName = groupName };
            uow.CommitChanges();
        }
    }

    #region Schedules
    public static bool EnableSmsGenerator = false;
    public static bool EnableMailGenerator = false;
    public static bool EnableSmsSender = false;
    public static bool EnableMailSender = false;
    #endregion

    #region Main
    public const string ReportCustomDateTimeFormat = "yyyy.MMM.dd HH:mm";
    public static string MyTempName => DateTime.Now.ToString("MMMddHHmmssfff");
    public static string GeteratedFilesName;
    #endregion

    #region SmsConfiguration
    public static string SmsBaseUrl;

    public static string SmsClientUsername;
    public static string SmsClientPassword;
    public static string SmsClientID;
    public static string SmsServiceID;
    public static string SmsSenderName;
    public static string SmsSuccessCode;
    #endregion

    #region EMailConfiguration
    public static string SmtpServer;
    public static string SmtpUserName;
    public static string SmtpPassword;
    public static string EMailFrom;
    public static bool EnableSsl;
    public static bool UseDefaultCredentials;
    public static int SmtpPort;
    #endregion
}
