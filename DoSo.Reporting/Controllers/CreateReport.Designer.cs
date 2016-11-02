namespace DoSo.Reporting.Controllers
{
    partial class CreateReport
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
            this.SimpleAction_NewReport = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // SimpleAction_NewReport
            // 
            this.SimpleAction_NewReport.Caption = "NewReport";
            this.SimpleAction_NewReport.ConfirmationMessage = null;
            this.SimpleAction_NewReport.Id = "NewReport";
            this.SimpleAction_NewReport.ImageName = "NewReport";
            this.SimpleAction_NewReport.IsExecuting = false;
            this.SimpleAction_NewReport.TargetViewNesting = DevExpress.ExpressApp.Nesting.Root;
            this.SimpleAction_NewReport.ToolTip = null;
            this.SimpleAction_NewReport.TypeOfView = typeof(DevExpress.ExpressApp.View);
            this.SimpleAction_NewReport.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.NewReport_Execute);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction SimpleAction_NewReport;
    }
}
