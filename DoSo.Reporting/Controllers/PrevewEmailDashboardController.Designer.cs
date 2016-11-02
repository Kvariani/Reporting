namespace DoSo.Reporting.Controllers
{
    partial class PrevewEmailDashboardController
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
            this.simpleAction_PrevewDashboard = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // simpleAction_PrevewDashboard
            // 
            this.simpleAction_PrevewDashboard.Caption = "PrevewDashboard ";
            this.simpleAction_PrevewDashboard.ConfirmationMessage = null;
            this.simpleAction_PrevewDashboard.Id = "simpleAction_PrevewDashboard ";
            this.simpleAction_PrevewDashboard.ToolTip = null;
            this.simpleAction_PrevewDashboard.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.simpleAction_PrevewDashboard_Execute);
            // 
            // PrevewEmailDashboardController
            // 
            this.Actions.Add(this.simpleAction_PrevewDashboard);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction simpleAction_PrevewDashboard;
    }
}
