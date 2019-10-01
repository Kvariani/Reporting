using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.DashboardWin;
using DoSo.Reporting.BusinessObjects;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.Data;
using DevExpress.DataAccess.Native.Data;
using System.ComponentModel;
using DevExpress.DataAccess;
using System.IO;
using System.Xml;

namespace DoSo.Reporting.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class AddDashboardToScheduleController : ObjectViewController<DetailView, DoSoReportSchedule>
    {
        public AddDashboardToScheduleController()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }

        private void simpleAction_AddDashboard_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var form = new DashboardDesignerForm();

            var xml = ViewCurrentObject.Dashboard?.Xml;

            if (string.IsNullOrWhiteSpace(xml))
                CreateNewDashboard(form);
            else
                using (var ms = new MemoryStream())
                {
                    using (var sr = new StreamWriter(ms, Encoding.Default))
                    {
                        var doc = new XmlDocument();
                        doc.LoadXml(xml);
                        var definitionXml = doc.OuterXml;
                        sr.Write(definitionXml);
                        sr.Flush();
                        ms.Position = 0;
                        CreateNewDashboard(form);
                        form.dashboardDesigner1.LoadDashboard(ms);

                    }
                }

            form.ShowDialog();
        }

        void CreateNewDashboard(DashboardDesignerForm form)
        {
            //form.dashboardDesigner1.ConfigureDataConnection += DashboardDesigner1_ConfigureDataConnection;
            form.dashboardDesigner1.DashboardSaving += DashboardDesigner1_DashboardSaving;
            form.dashboardDesigner1.DataSourceWizardSettings.EnableCustomSql = true;
            form.dashboardDesigner1.DataSourceWizardSettings.AvailableDataSourceTypes = DashboardDesignerDataSourceType.Sql | DashboardDesignerDataSourceType.Excel;
            //form.dashboardDesigner1.DataSourceWizardSettings.AvailableSqlDataProviders = DashboardSqlDataProvider.MSSqlServer;
            //form.dashboardDesigner1.DataSourceWizardSettings.DisableNewConnections = false;
            form.dashboardDesigner1.DataSourceWizardSettings.ShowConnectionsFromAppConfig = true;
            //var dsDet = new DataSource() { Data = ViewCurrentObject.SqlDataSource.Result[0] };
            //var ds = new DashboardSqlDataSource(ViewCurrentObject.SqlDataSource.ConnectionParameters);
            //form.dashboardDesigner1.Dashboard.DataSources.Add(ds);
            //form.dashboardDesigner1.Dashboard.AddDataSource("asdf2", ViewCurrentObject.SqlDataSource.Result[0]);
            //form.dashboardDesigner1.DataSourceWizardSettings.DisableNewConnections = true;
            //da.DataSources.Add(ar);
            form.editExtractSourceConnectionBarItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            //form.newDataSourceBarItem1.ItemClick += NewDataSourceBarItem1_ItemClick;
            //form.newDataSourceBarItem1.ItemPress += NewDataSourceBarItem1_ItemPress;
            //form.dashboardDesigner1.SelectedDataSource = ds;
            //da.DataSources.Add(new DashboardDataSource() { DataSource = ViewCurrentObject.SqlDataSource });
            //da.AddDataSource("DefaultDataSource", ViewCurrentObject.SqlDataSource.Result[0]);
        }

        private void DashboardDesigner1_DashboardSaving(object sender, DashboardSavingEventArgs e)
        {
            e.Handled = true;
            using (var ms = new MemoryStream())
            {
                e.Dashboard.SaveToXml(ms);
                ms.Position = 0;
                using (var sr = new StreamReader(ms, Encoding.UTF8))
                {
                    var xml = sr.ReadToEnd();
                    if (ViewCurrentObject.Dashboard == null)
                        new DoSoDashboard(ViewCurrentObject.Session) { Xml = xml, Name = ViewCurrentObject.ScheduleDescription ?? $"Dashboard For Schedule - {ViewCurrentObject.ID}" };
                    else
                        ViewCurrentObject.Dashboard.Xml = xml;
                    ObjectSpace.CommitChanges();
                }
            }
        }

        private void Form_HandleCreated(object sender, EventArgs e)
        {

        }
    }
    public class DashboardDataSource : IDashboardDataSource
    {
        public CalculatedFieldCollection CalculatedFields
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public DataComponentBase DataSource;

        public string ComponentName
        {
            get
            {
                return "asdf";
            }

            set
            {

            }
        }

        public object Data
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public DataProcessingMode DataProcessingMode
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public IDataProvider DataProvider
        {
            get;
            set;
        }

        public string Filter
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public bool HasDataProvider
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsConnected
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsServerModeSupported
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Name
        {
            get
            {
                return "kjahsdf";
            }

            set
            {

            }
        }

        public OlapDataProvider OlapDataProvider
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<DevExpress.Data.IParameter> Parameters
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Prefix
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ISite Site
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public SqlDataProvider SqlDataProvider
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public event EventHandler Disposed;
        public event EventHandler<NameChangingEventArgs> NameChanging;

        public void BeginInit()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void EndInit()
        {
            throw new NotImplementedException();
        }

        public ICalculatedFieldsController GetCalculatedFieldsController()
        {
            throw new NotImplementedException();
        }

        public IDashboardDataSourceInternal GetDataSourceInternal()
        {
            return null;
        }

        public IDataSourceSchema GetDataSourceSchema(string dataMember)
        {
            throw new NotImplementedException();
        }

        public Type GetFieldSourceType(string name, string dataMember)
        {
            throw new NotImplementedException();
        }
    }
}
