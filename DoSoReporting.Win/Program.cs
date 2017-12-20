using System;
using System.Configuration;
using System.Windows.Forms;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Win;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using DevExpress.DataAccess.Sql;

namespace DoSoReporting.Win
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
#if EASYTEST
            DevExpress.ExpressApp.Win.EasyTest.EasyTestRemotingRegistration.Register();
#endif
            SqlDataSource.DisableCustomQueryValidation = true;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            EditModelPermission.AlwaysGranted = System.Diagnostics.Debugger.IsAttached;
            Tracing.LocalUserAppDataPath = Application.LocalUserAppDataPath;
            Tracing.Initialize();
            DoSoReportingWindowsFormsApplication winApplication = new DoSoReportingWindowsFormsApplication();

            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
            // Refer to the https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112680.aspx help article for more details on how to provide a custom splash form.
            //winApplication.SplashScreen = new DevExpress.ExpressApp.Win.Utils.DXSplashScreen("YourSplashImage.png");
            if (ConfigurationManager.ConnectionStrings["ConnectionString"] != null)
            {
                winApplication.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            }
#if EASYTEST
            if(ConfigurationManager.ConnectionStrings["EasyTestConnectionString"] != null) {
                winApplication.ConnectionString = ConfigurationManager.ConnectionStrings["EasyTestConnectionString"].ConnectionString;
            }
#endif
            if (System.Diagnostics.Debugger.IsAttached/* && winApplication.CheckCompatibilityType == CheckCompatibilityType.DatabaseSchema*/)
            {
                winApplication.DatabaseUpdateMode = DatabaseUpdateMode.UpdateDatabaseAlways;
            }

            //XpoDefault.DataLayer = XpoDefault.GetDataLayer(winApplication.ConnectionString, DevExpress.Xpo.DB.AutoCreateOption.SchemaAlreadyExists);
            XpoDefault.DataLayer = XpoDefault.GetDataLayer(winApplication.ConnectionString, DevExpress.Xpo.DB.AutoCreateOption.SchemaAlreadyExists);

            System.Threading.Tasks.Task.Run(() => HS.InitializeConfigItems());

            try
            {
                winApplication.Setup();
                winApplication.Start();
            }
            catch (Exception e)
            {
                winApplication.HandleException(e);
            }
        }

        private static void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            
        }
    }
}
