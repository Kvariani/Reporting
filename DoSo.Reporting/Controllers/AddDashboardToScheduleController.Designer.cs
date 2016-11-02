namespace DoSo.Reporting.Controllers
{
    partial class AddDashboardToScheduleController
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
            this.simpleAction_AddDashboard = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // simpleAction_AddDashboard
            // 
            this.simpleAction_AddDashboard.Caption = "AddDashboard";
            this.simpleAction_AddDashboard.ConfirmationMessage = null;
            this.simpleAction_AddDashboard.Id = "simpleAction_AddDashboard";
            this.simpleAction_AddDashboard.ToolTip = null;
            this.simpleAction_AddDashboard.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.simpleAction_AddDashboard_Execute);
            // 
            // AddDashboardToScheduleController
            // 
            this.Actions.Add(this.simpleAction_AddDashboard);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction simpleAction_AddDashboard;
    }
}
