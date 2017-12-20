using DevExpress.Data.Filtering;
using DevExpress.Xpo;
//using DevExpress.Xpo.DB;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Install;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Exceptions;
//using PostgreSqlConnectionProviderEx;
using ServiceBase = System.ServiceProcess.ServiceBase;
using Timer = System.Threading.Timer;
using DoSo.MessageSendService;
using DoSo.Reporting.Senders;
using DoSo.Reporting.Generators;
using DoSo.Reporting.BusinessObjects;
using DevExpress.DataAccess.Sql;

namespace DoSoMessageSendService
{
    class Program : ServiceBase
    {
        private static Timer mailGeneratorTimer;
        private static Timer smsGeneratorTimer;
        private static Timer mailSenderTimer;
        private static Timer smsSenderTimer;
        public static string InstallServiceName = "DoSoMessageSendingService";

        static void Main(string[] args)
        {
            SqlDataSource.DisableCustomQueryValidation = true;
            XpoDefault.TrackPropertiesModifications = true;
            //SafePostgreSqlConnectionProvider.Register();
            var connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            XpoDefault.DataLayer = XpoDefault.GetDataLayer(connectionString, AutoCreateOption.SchemaAlreadyExists);
            SimpleDataLayer.SuppressReentrancyAndThreadSafetyCheck = true;
            //{ "Reentrancy or cross thread operation detected. To suppress this exception, set DevExpress.Xpo.SimpleDataLayer.SuppressReentrancyAndThreadSafetyCheck = true"}

            var culture = new CultureInfo("en-US")
            {
                DateTimeFormat =
                {
                    ShortDatePattern = "dd.MM.yyyy",
                    LongDatePattern = "dd.MM.yyyy HH:mm",
                    LongTimePattern = "HH:mm"
                }
            };
            Thread.CurrentThread.CurrentCulture = culture;
            DevExpress.Utils.FormatInfo.AlwaysUseThreadFormat = true;
            //CultureInfo.DefaultThreadCurrentCulture = culture;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;

            var debugMode = Debugger.IsAttached;
            if (args.Length > 0)
            {
                for (var ii = 0; ii < args.Length; ii++)
                {
                    switch (args[ii].ToUpper())
                    {
                        case "/NAME":
                            if (args.Length > ii + 1)
                            {
                                InstallServiceName = args[++ii];
                            }
                            break;
                        case "/I":
                            InstallService();
                            return;
                        case "/U":
                            UninstallService();
                            return;
                        case "/D":
                            debugMode = true;
                            break;
                        default:
                            break;
                    }
                }
            }

            if (debugMode)
            {
                var service = new Program();
                service.OnStart(null);
                Console.WriteLine("Service Started...");
                Console.WriteLine("<press any key to exit...>");
                Console.Read();
            }
            else
            {
                var program = new Program();
                ServiceBase.Run(program);
            }
        }

        private static void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            var ex = e.Exception;

            if (!(ex is UnableToOpenDatabaseException)) return;

            Environment.Exit(1);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //Environment.Exit(1);
        }

        

        void RunService()
        {

            var mailGeneratorThread = new Thread(StartMailGeneratorTread);
            mailGeneratorThread.SetApartmentState(ApartmentState.STA);

            var smsGeneratorThread = new Thread(StartSmsGeneratorThread);
            var emailSenderThred = new Thread(StartEmailSender);
            var smsSenderThread = new Thread(StartSmsSender);
            try
            {
                while (true)
                {
                    HS.GetConfiguration();

                    if (!mailGeneratorThread.CheckThreadState())
                        mailGeneratorThread.Start();

                    if (!smsGeneratorThread.CheckThreadState())
                        smsGeneratorThread.Start();

                    if (!emailSenderThred.CheckThreadState())
                        emailSenderThred.Start();

                    if (!smsSenderThread.CheckThreadState())
                        smsSenderThread.Start();

                    Thread.Sleep(10000);
                }
            }
            catch (Exception ex)
            {
                HS.CreateExceptionLog(ex.Message, ex.ToString(), 10);
                RunService();
            }
        }

        void StartSmsSender(object state)
        {
            while (true)
            {
                if (HS.EnableSmsSender)
                    SmsSender.SendAll();
                Thread.Sleep(1000);
            }
        }

        void StartEmailSender(object state)
        {
            while (true)
            {
                if (HS.EnableMailSender)
                    MailSender.SendAll();
                Thread.Sleep(1000);
            }
        }

        void StartSmsGeneratorThread(object state)
        {
            while (true)
            {
                if (HS.EnableSmsGenerator)
                    SmsGenerator.GenerateAll();
                Thread.Sleep(1000);
            }
        }


        private void StartMailGeneratorTread(object state)
        {
            while (true)
            {
                if (HS.EnableMailGenerator)
                    MailGenerator.GenerateAll();
                Thread.Sleep(1000);
            }
        }

        #region ServiceInstaller


        protected override void OnStart(string[] args)
        {
            RunService();
        }
        protected override void OnStop()
        {

        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
        private static bool IsServiceInstalled()
        {
            return ServiceController.GetServices().Any(s => s.ServiceName == InstallServiceName);
        }
        private static void InstallService()
        {
            if (IsServiceInstalled())
                UninstallService();

            ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
        }

        private static void UninstallService()
        {
            ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
        }
        #endregion
    }
}