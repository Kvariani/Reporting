namespace DoSo.Reporting.Controllers
{
    partial class GenerateMessagesController
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            simpleAction_GenerateMessages = new DevExpress.ExpressApp.Actions.SimpleAction(this.components)
            {
                Caption = "GenerateMessages",
                ConfirmationMessage = null,
                Id = "simpleAction_GenerateSms",
                ToolTip = null
            };

            simpleAction_GenerateMessages.Execute += SimpleAction_GenerateMessages_Execute;
        }

        private DevExpress.ExpressApp.Actions.SimpleAction simpleAction_GenerateMessages;
    }
}
