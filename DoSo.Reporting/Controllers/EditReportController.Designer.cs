namespace DoSo.Reporting.Controllers
{
    partial class EditReportController
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
            this.simpleAction_EditReport = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // simpleAction_EditReport
            // 
            this.simpleAction_EditReport.Caption = "EditReport";
            this.simpleAction_EditReport.ConfirmationMessage = null;
            this.simpleAction_EditReport.Id = "simpleAction_EditReport";
            this.simpleAction_EditReport.ToolTip = null;
            this.simpleAction_EditReport.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.simpleAction_EditReport_Execute);
            // 
            // EditReportController
            // 
            this.Actions.Add(this.simpleAction_EditReport);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction simpleAction_EditReport;
    }
}
