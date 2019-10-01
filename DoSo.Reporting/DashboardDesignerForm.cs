using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.DataSourceWizard;
using DevExpress.DashboardWin;
using DevExpress.DashboardWin.DataSourceWizard;
using DevExpress.DashboardWin.Native;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.Data;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.UI.Wizard;
using DevExpress.DataAccess.UI.Wizard.Services;
using DevExpress.DataAccess.UI.Wizard.Views;
using DevExpress.DataAccess.Wizard;
//using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Presenters;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.DataAccess.Wizard.Views;
//using DevExpress.Entity.ProjectModel;
//using DevExpress.DataAccess.Wizard.Native;
//using DevExpress.DataAccess.Wizard.Services;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoSo.Reporting
{
    public partial class DashboardDesignerForm : XtraForm
    {
        public DashboardDesignerForm()
        {
            InitializeComponent();

            this.dashboardDesigner1.Dashboard.ValidateCustomSqlQuery += Dashboard_ValidateCustomSqlQuery;
            this.dashboardDesigner1.DataSourceWizardCustomizationService = new DataSourceWizardCustomizationService();
            dashboardDesigner1.DataSourceWizardCustomization = new DataSourceWizardCustomization();
            this.dashboardDesigner1.DataSourceWizard.SqlWizardSettings.DisableNewConnections = true;
            this.dashboardDesigner1.DataSourceWizard.SqlWizardSettings.EnableCustomSql = true;
            this.dashboardDesigner1.DataSourceWizard.SqlWizardSettings.QueryBuilderLight = true;
            this.dashboardDesigner1.DataSourceWizard.SqlWizardSettings.ToSqlWizardOptions();
        }

        private void Dashboard_ValidateCustomSqlQuery(object sender, DevExpress.DashboardCommon.ValidateDashboardCustomSqlQueryEventArgs e)
        {

        }

        private void DashboardDesigner1_DashboardChanged(object sender, EventArgs e)
        {

        }
    }
    public class DataSourceWizardCustomization : IDashboardDataSourceWizardCustomization
    {
        public void CustomizeDataSourceWizard(IWizardCustomization<DashboardDataSourceModel> customization)
        {
            //customization.StartPage = typeof(ChooseConnectionPage<DashboardDataSourceForm>);
            //customization.Model.DataSourceType = DashboardDataSourceType.Xpo;
            customization.RegisterPage<ConfigureQueryPage<DashboardDataSourceModel>, CustomConfigureQueryPage<DashboardDataSourceModel>>();
            //CustomConfigureQueryPage
            //customization.RegisterPageView<IConfigureQueryPageView, CustomConfigureQueryPageView>();
        }
    }

    public class CustomConfigureQueryPage<TModel> : ConfigureQueryPage<TModel> where TModel : DashboardDataSourceModel
    {

        //public CustomConfigureQueryPage(IConfigureQueryPageView view, IWizardRunnerContext context, SqlWizardOptions options,
        //    IDBSchemaProvider dbSchemaProvider, IParameterService parameterService, ICustomQueryValidator customQueryValidator)
        //    : base(view, context, options,
        //     dbSchemaProvider, parameterService, customQueryValidator)
        //{ }

        public CustomConfigureQueryPage(IConfigureQueryPageView view, IWizardRunnerContext context, SqlWizardOptions options, IDBSchemaProviderEx dbSchemaProviderEx, IParameterService parameterService, ICustomQueryValidator customQueryValidator) : base(view, context, options, dbSchemaProviderEx, parameterService, customQueryValidator) { }

        protected override void RunQueryBuilder()
        {
            base.RunQueryBuilder();
        }

        public override bool FinishEnabled => true;

        public override bool Validate(out string errorMessage)
        {
            var a =  base.Validate(out errorMessage);
            return a;
        }


        public override void Begin()
        {
            base.Begin();
            //((CustomConfigureQueryPageView)View).SetSqlString("Select * from Orders");
        }
    }

    public class DataSourceWizardCustomizationService : IDataSourceWizardCustomizationService
    {
        public void CustomizeDataSourceWizard(IWizardCustomization<DashboardDataSourceModel> customization)
        {
            //customization.RegisterPageView<IConfigureOlapParametersPageView, CustomConfigureOlapParametersPageView>();
            //customization.RegisterPageView<IChooseDataSourceTypePageView, DashboardChooseDataSourceTypePagei>();
            ////customization.register<IConnectionStorageService, ConnectionStorageService1>();
            //customization.RegisterPageView<IConfigureExtractDataSourcePageView, ConfigureExtractDataSourcePageView1>();

            customization.RegisterPageView<IConfigureParametersPageView, ConfigureParametersPageView1>();
            customization.RegisterPageView<IMultiQueryConfigurePageView, MultiQueryConfigurePageViewEx>();
            customization.RegisterPageView<IConfigureQueryPageView, ConfigureQueryPageViewEx>();

            //tool.RegisterPage<ConfigureQueryPage<TModel>, MyConfigureQueryPage<TModel>>();
            //tool.RegisterPageView<IConfigureQueryPageView, MyConfigureQueryPageView>();


            // Register the modified "Select the data source type" wizard page.
            //customization.RegisterPage<CustomChooseDataSourceTypePage, CustomChooseDataSourceTypePage>();
            //ConfigureQueryPageView
        }
    }

    //public class CustomChooseDataSourceNamePage : ChooseDataSourceNamePage<DataSourceModel>
    //{
    //    public CustomChooseDataSourceNamePage(IChooseDataSourceNamePageView view, IDataSourceNameCreationService dataSourceNameCreator)
    //        : base(view, dataSourceNameCreator) { }

    //    // Override the GetNextPageType method to open the modified "Select the data source type" page when an end-user clicks "Next". 
    //    public override Type GetNextPageType()
    //    {
    //        return null;
    //        //return typeof(CustomChooseDataSourceTypePage);
    //    }
    //}


    class ConfigureQueryPageViewEx : ConfigureQueryPageView
    {
        public ConfigureQueryPageViewEx(IDisplayNameProvider displayNameProvider, IServiceProvider propertyGridServices, ICustomQueryValidator customQueryValidator, SqlWizardOptions options) : base(displayNameProvider, propertyGridServices, customQueryValidator, options)
        {
            EnableNext(false);
            EnableFinish(true);
            Finish += ConfigureQueryPageViewEx_Finish1;
            Next += ConfigureQueryPageViewEx_Next;
            this.Finish += ConfigureQueryPageViewEx_Finish;
            //this.buttonFinish.EnabledChanged += ButtonFinish_EnabledChanged;
        }

        public override bool ValidateChildren()
        {
            return base.ValidateChildren();
        }

        private void ConfigureQueryPageViewEx_Finish1(object sender, EventArgs e)
        {
            
        }

        private void ConfigureQueryPageViewEx_Next(object sender, EventArgs e)
        {

        }

        private void ButtonFinish_EnabledChanged(object sender, EventArgs e)
        {
            (sender as DevExpress.XtraEditors.SimpleButton).Enabled = true;
            EnableFinish(true);

        }

        private void ConfigureQueryPageViewEx_Finish(object sender, EventArgs e)
        {

        }
    }

    class MultiQueryConfigurePageViewEx : MultiQueryConfigurePageView
    {
        public MultiQueryConfigurePageViewEx()
        {
        }
    }

    class ConfigureParametersPageView1 : ConfigureParametersPageView
    {
        public ConfigureParametersPageView1(IServiceProvider propertyGridServices, IParameterService parameterService, IRepositoryItemsProvider repositoryItemsProvider) : base(propertyGridServices, parameterService, repositoryItemsProvider)
        {

        }

        public override bool ConfirmQueryExecution()
        {
            return false;
            return base.ConfirmQueryExecution();
        }
    }

    class DashboardChooseDataSourceTypePagei : ChooseDataSourceTypePageView
    {
        public DashboardChooseDataSourceTypePagei()
        {
            this.EnableFinish(true);
            this.buttonFinish.Enabled = true;
            this.buttonFinish.EnabledChanged += ButtonFinish_EnabledChanged;

        }

        private void ButtonFinish_EnabledChanged(object sender, EventArgs e)
        {
            (sender as DevExpress.XtraEditors.SimpleButton).Enabled = true;
        }
    }

    //class ConnectionStorageService1 : ConnectionStorageService
    //{
    //    public ConnectionStorageService1()
    //    {
    //        var a = this.GetConnections();
    //    }
    //}
    class ConfigureExtractDataSourcePageView1 : ConfigureExtractDataSourcePageView
    {
        //protected override OlapConnectionParametersControl CreateOlapConnectionParametersControl()
        //{
        //    return new CustomOlapConnectionParametersControl();
        //}
    }

    class CustomConfigureOlapParametersPageView : ConfigureOlapParametersPageView
    {
        protected override OlapConnectionParametersControl CreateOlapConnectionParametersControl()
        {
            return new CustomOlapConnectionParametersControl();
        }
    }

    class CustomOlapConnectionParametersControl : OlapConnectionParametersControl
    {
        public CustomOlapConnectionParametersControl() : base()
        {
            lciConnectionType.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
        }
    }
}
