namespace DoSo.Reporting.Controllers
{
    partial class PrevewEmailReportController
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
            this.simpleAction_PrevewReport = new DevExpress.ExpressApp.Actions.SimpleAction();
            // 
            // simpleAction_PrevewReport
            // 
            this.simpleAction_PrevewReport.Caption = "PrevewReport";
            this.simpleAction_PrevewReport.ConfirmationMessage = null;
            this.simpleAction_PrevewReport.Id = "simpleAction_PrevewReport";
            this.simpleAction_PrevewReport.ToolTip = null;
            this.simpleAction_PrevewReport.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.simpleAction_PrevewReport_Execute);
            // 
            // PrevewEmailReportController
            // 
            this.Actions.Add(this.simpleAction_PrevewReport);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction simpleAction_PrevewReport;
    }
}
