namespace DoSo.Reporting.Controllers
{
    partial class PrevewMessagesController
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
            components = new System.ComponentModel.Container();
            popupWindowShowAction_PrevewMessages = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components)
            {
                AcceptButtonCaption = null,
                CancelButtonCaption = null,
                Caption = "PrevewMessages",
                ConfirmationMessage = null,
                Id = "popupWindowShowAction_PrevewMessages",
                ToolTip = null,
            };

            simpleAction_PrevewMessages = new DevExpress.ExpressApp.Actions.SimpleAction(components)
            {
                Id = nameof(simpleAction_PrevewMessages)
            };
            simpleAction_PrevewMessages.Execute += SimpleAction_PrevewMessages_Execute;


            popupWindowShowAction_PrevewMessages.Execute += PopupWindowShowAction_PrevewMessages_Execute;

            Actions.Add(this.popupWindowShowAction_PrevewMessages);
            Actions.Add(simpleAction_PrevewMessages);

        }


        #endregion

        private DevExpress.ExpressApp.Actions.PopupWindowShowAction popupWindowShowAction_PrevewMessages;
        private DevExpress.ExpressApp.Actions.SimpleAction simpleAction_PrevewMessages;
    }
}
