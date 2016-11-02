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

        //private static DateTime failedLogTime;
        //private static Type lastExceptionType;

        private static void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            var ex = e.Exception;

            if (!(ex is UnableToOpenDatabaseException)) return;

            //GeneratorHelper.CreateLogFileWithException("Connection Lost - " + ex);
            Environment.Exit(1);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //Environment.Exit(1);
        }

        void RunService()
        {
            try
            {
                var mailGeneratorThread = new Thread(StartMailGeneratorTread);
                mailGeneratorThread.SetApartmentState(ApartmentState.STA);

                while (true)
                {
                    HS.GetConfiguration();

                    if (mailGeneratorThread.ThreadState != System.Threading.ThreadState.Running)
                    {
                        mailGeneratorThread.Start();
                    }
                    Thread.Sleep(10000);
                }

                //mailGeneratorTimer = new Timer(_ => MailGenerator.GenerateAll(mailGeneratorTimer), null, 1000, Timeout.Infinite);
                //smsGeneratorTimer = new Timer(_ => SmsGenerator.GenerateAll(smsGeneratorTimer), null, 1000, Timeout.Infinite);
                //mailSenderTimer = new Timer(_ => MailSender.SendAll(mailSenderTimer), null, 1000, Timeout.Infinite);
                //smsSenderTimer = new Timer(_ => SmsSender.SendAll(smsSenderTimer), null, 1000, Timeout.Infinite);
            }
            catch (Exception ex)
            {
                //GeneratorHelper.CreateLogFileWithException(ex.ToString());
            }
        }

        public void LoadConfiguration()
        {

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