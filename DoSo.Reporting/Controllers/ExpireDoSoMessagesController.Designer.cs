namespace DoSo.Reporting.Controllers
{
    partial class ExpireDoSoMessagesController
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
            this.simpleAction_ExpireMessages = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // simpleAction_ExpireMessages
            // 
            this.simpleAction_ExpireMessages.Caption = "ExpireMessages";
            this.simpleAction_ExpireMessages.ConfirmationMessage = null;
            this.simpleAction_ExpireMessages.Id = "simpleAction_ExpireMessages";
            this.simpleAction_ExpireMessages.ToolTip = null;
            this.simpleAction_ExpireMessages.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.simpleAction_ExpireMessages_Execute);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction simpleAction_ExpireMessages;
    }
}
