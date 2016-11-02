namespace DoSo.Reporting.Controllers
{
    partial class EditDashboardController
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
            this.simpleAction_EditDashboard = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.simpleAction_PreviwDashboard = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // simpleAction_EditDashboard
            // 
            this.simpleAction_EditDashboard.Caption = "EditDashboard";
            this.simpleAction_EditDashboard.ConfirmationMessage = null;
            this.simpleAction_EditDashboard.Id = "simpleAction_EditDashboard";
            this.simpleAction_EditDashboard.ToolTip = null;
            this.simpleAction_EditDashboard.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.simpleAction_EditDashboard_Execute);
            // 
            // simpleAction_PreviwDashboard
            // 
            this.simpleAction_PreviwDashboard.Caption = "Preview";
            this.simpleAction_PreviwDashboard.ConfirmationMessage = null;
            this.simpleAction_PreviwDashboard.Id = "simpleAction_PreviwDashboard";
            this.simpleAction_PreviwDashboard.ToolTip = null;
            this.simpleAction_PreviwDashboard.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.simpleAction_PreviwDashboard_Execute);
            // 
            // EditDashboardController
            // 
            this.Actions.Add(this.simpleAction_EditDashboard);
            this.Actions.Add(this.simpleAction_PreviwDashboard);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction simpleAction_EditDashboard;
        private DevExpress.ExpressApp.Actions.SimpleAction simpleAction_PreviwDashboard;
    }
}
