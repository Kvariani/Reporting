using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.DataAccess.Sql;
using DevExpress.XtraReports.Wizards3;
using DevExpress.DataAccess.Wizard.Model;
using System.ComponentModel.Design;
using DevExpress.DataAccess.UI.Wizard;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.DataAccess.Wizard.Presenters;
using DevExpress.DataAccess.UI.Wizard.Views;
using DevExpress.DataAccess.Wizard;
using DevExpress.Entity.ProjectModel;
using DoSo.Reporting.BusinessObjects.Base;
using DevExpress.XtraBars.Ribbon;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.DataAccess;
using DevExpress.Data;
using DevExpress.DataAccess.UI.Sql;

namespace DoSoReporting.Module.Controllers
{
    public partial class ScheduleDataSourceController : ObjectViewController<DetailView, DoSoScheduleBase>
    {
        public ScheduleDataSourceController()
        {
            InitializeComponent();
        }

        private void simpleAction_AddDataSource_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            ObjectSpace.CommitChanges();

            var dataSourceBase = InitializeWizard();
            if (dataSourceBase == null)
                return;

            ViewCurrentObject.DataSourceXml = dataSourceBase.SaveToXml().ToString();

            var cu = View.CurrentObject as DoSoScheduleBase;
            if (cu != null)
                cu.DataSource = dataSourceBase;

            ObjectSpace.CommitChanges();
        }

        public DataComponentBase InitializeWizard()
        {
            var designForm = new XRDesignFormEx();
            //designForm.DesignPanel.CreateControl();
            designForm.DesignPanel.DesignerHostLoading += DesignPanel_DesignerHostLoading;
            designForm.DesignPanel.ExecCommand(ReportCommand.NewReportWizard);

            return designForm.DesignPanel.Report.DataSource as DataComponentBase;
        }

        private void SimpleAction_EditDataSource_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            //var view = new ConfigureQueryPageViewEx(null, null, null, SqlWizardOptions.EnableCustomSql);
            //view.CreateControl();
            //view.Show();
            if (ViewCurrentObject.SqlDataSource != null)
            {
                var result = ViewCurrentObject.SqlDataSource.Queries[0].EditQuery();
                if (result)
                    ViewCurrentObject.DataSourceXml = ViewCurrentObject.SqlDataSource.SaveToXml().ToString();
            }
        }

        private void DesignPanel_DesignerHostLoading(object sender, EventArgs e)
        {
            (sender as XRDesignPanel).AddService(typeof(IWizardCustomizationService), new WizardCustomizationService());
        }
    }

    //public class XRDesignRibbonFormEx : XRDesignRibbonForm
    //{

    //    public XRDesignRibbonFormEx()
    //    {
    //        custom
    //    }
    //}

    public class WizardCustomizationService : IWizardCustomizationService
    {
        bool IWizardCustomizationService.TryCreateDataSource(IDataSourceModel model, out object dataSource, out string dataMember)
        {
            dataMember = string.Empty;
            dataSource = null;
            return false;
        }

        public void CustomizeReportWizard(IWizardCustomization<XtraReportModel> tool)
        {

            tool.StartPage = typeof(CustomChooseDataSourceTypePage);
            tool.RegisterPage<CustomChooseDataSourceTypePage, CustomChooseDataSourceTypePage>();
            tool.RegisterPageView<IChooseDataSourceTypePageView, CustomChooseDataSourceTypePageView>();
            tool.RegisterPage<CustomQueryPage, CustomQueryPage>();
            //tool.RegisterPageView<CustomQueryPage, CustomQueryPage>();
            tool.RegisterPageView<IConfigureQueryPageView, ConfigureQueryPageViewEx>();
        }

        public void CustomizeDataSourceWizard(IWizardCustomization<XtraReportModel> tool)
        {

        }

        public bool TryCreateReport(IDesignerHost designerHost, XtraReportModel model, object dataSource, string dataMember)
        {
            return false;
        }
    }

    public class CustomQueryPage : ConfigureQueryPage<XtraReportModel>
    {
        public CustomQueryPage(IConfigureQueryPageView view, IWizardRunnerContext context, SqlWizardOptions options, IDBSchemaProvider dbSchemaProvider, IParameterService parameterService, ICustomQueryValidator customQueryValidator) : base(view, context, options, dbSchemaProvider, parameterService, customQueryValidator)
        {

        }

        public override Type GetNextPageType()
        {
            var a = base.GetNextPageType();
            return a;
        }
    }

    public class CustomChooseDataSourceTypePage : ChooseDataSourceTypePage<XtraReportModel>
    {
        public CustomChooseDataSourceTypePage(IChooseDataSourceTypePageView view, IWizardRunnerContext context, IEnumerable<SqlDataConnection> dataConnections, ISolutionTypesProvider solutionTypesProvider, SqlWizardOptions options)
            : base(view, context, dataConnections, solutionTypesProvider, options) { }
        public override Type GetNextPageType()
        {
            return base.GetNextPageType();
        }
    }


    public class ConfigureQueryPageViewEx : ConfigureQueryPageView
    {
        public ConfigureQueryPageViewEx(IDisplayNameProvider displayNameProvider, IServiceProvider propertyGridServices, ICustomQueryValidator customQueryValidator, SqlWizardOptions options) : base(displayNameProvider, propertyGridServices, customQueryValidator, SqlWizardOptions.EnableCustomSql)
        {
            
            
        }

    }

    public class CustomChooseDataSourceTypePageView : ChooseDataSourceTypePageView
    {
        public CustomChooseDataSourceTypePageView(DataSourceTypes dataSourceTypes) : base(dataSourceTypes)
        {
        }
        protected override void InitializeGallery(GalleryItemGroup galleryItemGroup)
        {
            base.InitializeGallery(galleryItemGroup);
            galleryItemGroup.Items.RemoveAt(1);
            galleryItemGroup.Items.RemoveAt(1);
        }
    }
}
