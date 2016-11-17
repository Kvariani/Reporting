namespace DoSoReporting.Module.Controllers
{
    partial class SpreadSheetController
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
            this.simpleAction_OpenWorkBook = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // simpleAction1
            // 
            simpleAction_OpenWorkBook.Caption = null;
            simpleAction_OpenWorkBook.ConfirmationMessage = null;
            simpleAction_OpenWorkBook.ImageName = "OpenWorkBook";
            simpleAction_OpenWorkBook.Id = "simpleAction_OpenWorkBook";
            simpleAction_OpenWorkBook.Caption = "OpenWorkBook";
            simpleAction_OpenWorkBook.ToolTip = null;
            simpleAction_OpenWorkBook.Execute += SimpleAction_OpenWorkBook_Execute;
            // 
            // SpreadSheetController
            // 
            this.Actions.Add(this.simpleAction_OpenWorkBook);

        }


        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction simpleAction_OpenWorkBook;
    }
}
