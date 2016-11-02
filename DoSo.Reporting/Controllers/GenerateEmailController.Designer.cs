namespace DoSo.Reporting.Controllers
{
    partial class GenerateEmailController
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
            this.simpleAction_GenerateEmails = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // simpleAction_GenerateEmails
            // 
            this.simpleAction_GenerateEmails.Caption = "simpleAction_GenerateEmails";
            this.simpleAction_GenerateEmails.ConfirmationMessage = null;
            this.simpleAction_GenerateEmails.Id = "simpleAction_GenerateEmails";
            this.simpleAction_GenerateEmails.ToolTip = null;
            this.simpleAction_GenerateEmails.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.simpleAction_GenerateEmails_Execute);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction simpleAction_GenerateEmails;
    }
}
