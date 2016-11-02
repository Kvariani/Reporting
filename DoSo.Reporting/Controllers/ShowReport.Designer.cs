namespace DoSo.Reporting.Controllers
{
    partial class ShowReport
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
            this.components = new System.ComponentModel.Container();
            this.simpleAction_ShowReport = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // simpleAction_ShowReport
            // 
            this.simpleAction_ShowReport.Caption = "რეპორტის დაგენერირება";
            this.simpleAction_ShowReport.Category = "ReportGenerationActionsContainer";
            this.simpleAction_ShowReport.ConfirmationMessage = null;
            this.simpleAction_ShowReport.Id = "simpleAction_ShowReport";
            this.simpleAction_ShowReport.IsExecuting = false;
            this.simpleAction_ShowReport.TargetObjectType = typeof(DoSo.Reporting.BusinessObjects.ReportExecution);
            this.simpleAction_ShowReport.ToolTip = null;
            this.simpleAction_ShowReport.TypeOfView = typeof(DevExpress.ExpressApp.View);
            this.simpleAction_ShowReport.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.simpleAction_ShowReport_Execute);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction simpleAction_ShowReport;
        //private DevExpress.ExpressApp.Actions.PopupWindowShowAction popupWindowShowAction_ShowReport;
    }
}
