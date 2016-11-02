namespace DoSo.Reporting.Controllers
{
    partial class AddReportToScheduleController
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
            this.simpleAction_AddReportToSchedule = new DevExpress.ExpressApp.Actions.SimpleAction();
            // 
            // simpleAction_AddReportToSchedule
            // 
            this.simpleAction_AddReportToSchedule.Caption = "AddReport";
            this.simpleAction_AddReportToSchedule.ConfirmationMessage = null;
            this.simpleAction_AddReportToSchedule.Id = "simpleAction_AddReportToSchedule";
            this.simpleAction_AddReportToSchedule.ToolTip = null;
            this.simpleAction_AddReportToSchedule.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.simpleAction_AddReportToSchedule_Execute);
            // 
            // AddReportToScheduleController
            // 
            this.Actions.Add(this.simpleAction_AddReportToSchedule);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction simpleAction_AddReportToSchedule;
    }
}
