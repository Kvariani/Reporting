namespace DoSoReporting.Win
{
    partial class DoSoReportingWindowsFormsApplication
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.module1 = new DevExpress.ExpressApp.SystemModule.SystemModule();
            this.module2 = new DevExpress.ExpressApp.Win.SystemModule.SystemWindowsFormsModule();
            this.module3 = new DoSoReporting.Module.DoSoReportingModule();
            this.module4 = new DoSoReporting.Module.Win.DoSoReportingWindowsFormsModule();
            //this.auditTrailModule = new DevExpress.ExpressApp.AuditTrail.AuditTrailModule();
            //this.objectsModule = new DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule();
            this.chartModule = new DevExpress.ExpressApp.Chart.ChartModule();
            this.chartWindowsFormsModule = new DevExpress.ExpressApp.Chart.Win.ChartWindowsFormsModule();
            //this.cloneObjectModule = new DevExpress.ExpressApp.CloneObject.CloneObjectModule();
            this.conditionalAppearanceModule = new DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule();
            //this.fileAttachmentsWindowsFormsModule = new DevExpress.ExpressApp.FileAttachments.Win.FileAttachmentsWindowsFormsModule();
            //this.htmlPropertyEditorWindowsFormsModule = new DevExpress.ExpressApp.HtmlPropertyEditor.Win.HtmlPropertyEditorWindowsFormsModule();
            //this.kpiModule = new DevExpress.ExpressApp.Kpi.KpiModule();
            //this.notificationsModule = new DevExpress.ExpressApp.Notifications.NotificationsModule();
            //this.notificationsWindowsFormsModule = new DevExpress.ExpressApp.Notifications.Win.NotificationsWindowsFormsModule();
            //this.pivotChartModuleBase = new DevExpress.ExpressApp.PivotChart.PivotChartModuleBase();
            //this.pivotChartWindowsFormsModule = new DevExpress.ExpressApp.PivotChart.Win.PivotChartWindowsFormsModule();
            //this.pivotGridModule = new DevExpress.ExpressApp.PivotGrid.PivotGridModule();
            //this.pivotGridWindowsFormsModule = new DevExpress.ExpressApp.PivotGrid.Win.PivotGridWindowsFormsModule();
            this.reportsModuleV2 = new DevExpress.ExpressApp.ReportsV2.ReportsModuleV2();
            this.reportsWindowsFormsModuleV2 = new DevExpress.ExpressApp.ReportsV2.Win.ReportsWindowsFormsModuleV2();
            //this.scriptRecorderModuleBase = new DevExpress.ExpressApp.ScriptRecorder.ScriptRecorderModuleBase();
            //this.scriptRecorderWindowsFormsModule = new DevExpress.ExpressApp.ScriptRecorder.Win.ScriptRecorderWindowsFormsModule();
            this.treeListEditorsModuleBase = new DevExpress.ExpressApp.TreeListEditors.TreeListEditorsModuleBase();
            this.treeListEditorsWindowsFormsModule = new DevExpress.ExpressApp.TreeListEditors.Win.TreeListEditorsWindowsFormsModule();
            this.validationModule = new DevExpress.ExpressApp.Validation.ValidationModule();
            this.validationWindowsFormsModule = new DevExpress.ExpressApp.Validation.Win.ValidationWindowsFormsModule();
            this.viewVariantsModule = new DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule();
            this.reportingModule1 = new DoSo.Reporting.ReportingModule();
            //this.reportsModule1 = new DevExpress.ExpressApp.Reports.ReportsModule();
            //this.reportsWindowsFormsModule1 = new DevExpress.ExpressApp.Reports.Win.ReportsWindowsFormsModule();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // auditTrailModule
            // 
            //this.auditTrailModule.AuditDataItemPersistentType = typeof(DevExpress.Persistent.BaseImpl.AuditDataItemPersistent);
            // 
            // notificationsModule
            // 
            //this.notificationsModule.CanAccessPostponedItems = false;
            //this.notificationsModule.NotificationsRefreshInterval = System.TimeSpan.Parse("00:05:00");
            //this.notificationsModule.NotificationsStartDelay = System.TimeSpan.Parse("00:00:05");
            //this.notificationsModule.ShowNotificationsWindow = true;
            // 
            // pivotChartModuleBase
            // 
            //this.pivotChartModuleBase.DataAccessMode = DevExpress.ExpressApp.CollectionSourceDataAccessMode.Client;
            //this.pivotChartModuleBase.ShowAdditionalNavigation = false;
            // 
            // reportsModuleV2
            // 
            this.reportsModuleV2.EnableInplaceReports = true;
            this.reportsModuleV2.ReportDataType = typeof(DevExpress.Persistent.BaseImpl.ReportDataV2);
            this.reportsModuleV2.ReportStoreMode = DevExpress.ExpressApp.ReportsV2.ReportStoreModes.XML;
            // 
            // validationModule
            // 
            this.validationModule.AllowValidationDetailsAccess = true;
            this.validationModule.IgnoreWarningAndInformationRules = false;
            // 
            // reportsModule1
            // 
            //this.reportsModule1.EnableInplaceReports = true;
            //this.reportsModule1.ReportDataType = typeof(DevExpress.Persistent.BaseImpl.ReportData);
            // 
            // DoSoReportingWindowsFormsApplication
            // 
            this.ApplicationName = "DoSoReporting";
            this.CheckCompatibilityType = DevExpress.ExpressApp.CheckCompatibilityType.DatabaseSchema;
            this.Modules.Add(this.module1);
            this.Modules.Add(this.module2);
            //this.Modules.Add(this.auditTrailModule);
            //this.Modules.Add(this.objectsModule);
            this.Modules.Add(this.chartModule);
            //this.Modules.Add(this.cloneObjectModule);
            this.Modules.Add(this.conditionalAppearanceModule);
            this.Modules.Add(this.validationModule);
            //this.Modules.Add(this.kpiModule);
            //this.Modules.Add(this.notificationsModule);
            //this.Modules.Add(this.pivotChartModuleBase);
            //this.Modules.Add(this.pivotGridModule);
            this.Modules.Add(this.reportsModuleV2);
            //this.Modules.Add(this.scriptRecorderModuleBase);
            this.Modules.Add(this.treeListEditorsModuleBase);
            this.Modules.Add(this.viewVariantsModule);
            this.Modules.Add(this.module3);
            this.Modules.Add(this.chartWindowsFormsModule);
            //this.Modules.Add(this.fileAttachmentsWindowsFormsModule);
            //this.Modules.Add(this.htmlPropertyEditorWindowsFormsModule);
            //this.Modules.Add(this.notificationsWindowsFormsModule);
            //this.Modules.Add(this.pivotChartWindowsFormsModule);
            //this.Modules.Add(new Common.Win.CommonWindowsFormsModule());
            //this.Modules.Add(this.pivotGridWindowsFormsModule);
            this.Modules.Add(this.reportsWindowsFormsModuleV2);
            //this.Modules.Add(this.scriptRecorderWindowsFormsModule);
            this.Modules.Add(this.treeListEditorsWindowsFormsModule);
            this.Modules.Add(this.validationWindowsFormsModule);
            this.Modules.Add(this.module4);
            this.Modules.Add(this.reportingModule1);
            //this.Modules.Add(this.reportsModule1);
            //this.Modules.Add(this.reportsWindowsFormsModule1);
            this.UseOldTemplates = false;
            this.DatabaseVersionMismatch += new System.EventHandler<DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs>(this.DoSoReportingWindowsFormsApplication_DatabaseVersionMismatch);
            this.CustomizeLanguagesList += new System.EventHandler<DevExpress.ExpressApp.CustomizeLanguagesListEventArgs>(this.DoSoReportingWindowsFormsApplication_CustomizeLanguagesList);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.ExpressApp.SystemModule.SystemModule module1;
        private DevExpress.ExpressApp.Win.SystemModule.SystemWindowsFormsModule module2;
        private DoSoReporting.Module.DoSoReportingModule module3;
        private DoSoReporting.Module.Win.DoSoReportingWindowsFormsModule module4;
        //private DevExpress.ExpressApp.AuditTrail.AuditTrailModule auditTrailModule;
        //private DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule objectsModule;
        private DevExpress.ExpressApp.Chart.ChartModule chartModule;
        private DevExpress.ExpressApp.Chart.Win.ChartWindowsFormsModule chartWindowsFormsModule;
        //private DevExpress.ExpressApp.CloneObject.CloneObjectModule cloneObjectModule;
        private DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule conditionalAppearanceModule;
        //private DevExpress.ExpressApp.FileAttachments.Win.FileAttachmentsWindowsFormsModule fileAttachmentsWindowsFormsModule;
        //private DevExpress.ExpressApp.HtmlPropertyEditor.Win.HtmlPropertyEditorWindowsFormsModule htmlPropertyEditorWindowsFormsModule;
        //private DevExpress.ExpressApp.Kpi.KpiModule kpiModule;
        //private DevExpress.ExpressApp.Notifications.NotificationsModule notificationsModule;
        //private DevExpress.ExpressApp.Notifications.Win.NotificationsWindowsFormsModule notificationsWindowsFormsModule;
        //private DevExpress.ExpressApp.PivotChart.PivotChartModuleBase pivotChartModuleBase;
        //private DevExpress.ExpressApp.PivotChart.Win.PivotChartWindowsFormsModule pivotChartWindowsFormsModule;
        //private DevExpress.ExpressApp.PivotGrid.PivotGridModule pivotGridModule;
        //private DevExpress.ExpressApp.PivotGrid.Win.PivotGridWindowsFormsModule pivotGridWindowsFormsModule;
        private DevExpress.ExpressApp.ReportsV2.ReportsModuleV2 reportsModuleV2;
        private DevExpress.ExpressApp.ReportsV2.Win.ReportsWindowsFormsModuleV2 reportsWindowsFormsModuleV2;
        //private DevExpress.ExpressApp.ScriptRecorder.ScriptRecorderModuleBase scriptRecorderModuleBase;
        //private DevExpress.ExpressApp.ScriptRecorder.Win.ScriptRecorderWindowsFormsModule scriptRecorderWindowsFormsModule;
        private DevExpress.ExpressApp.TreeListEditors.TreeListEditorsModuleBase treeListEditorsModuleBase;
        private DevExpress.ExpressApp.TreeListEditors.Win.TreeListEditorsWindowsFormsModule treeListEditorsWindowsFormsModule;
        private DevExpress.ExpressApp.Validation.ValidationModule validationModule;
        private DevExpress.ExpressApp.Validation.Win.ValidationWindowsFormsModule validationWindowsFormsModule;
        private DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule viewVariantsModule;
        private DoSo.Reporting.ReportingModule reportingModule1;
        //private DevExpress.ExpressApp.Reports.ReportsModule reportsModule1;
        //private DevExpress.ExpressApp.Reports.Win.ReportsWindowsFormsModule reportsWindowsFormsModule1;
    }
}
