namespace DoSoReporting.Module.Controllers
{
    partial class ScheduleDataSourceController
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
            simpleAction_AddDataSource = new DevExpress.ExpressApp.Actions.SimpleAction(this.components)
            {
                Caption = "AddDataSource",
                ConfirmationMessage = null,
                Id = "simpleAction_AddDataSource",
                ToolTip = null
            };
            simpleAction_AddDataSource.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.simpleAction_AddDataSource_Execute);

            simpleAction_EditDataSource = new DevExpress.ExpressApp.Actions.SimpleAction(this.components)
            {
                Caption = "EditDataSource",
                ConfirmationMessage = null,
                Id = "simpleAction_EditDataSource",
                ToolTip = null
            };
            simpleAction_EditDataSource.Execute += SimpleAction_EditDataSource_Execute;

            Actions.Add(simpleAction_AddDataSource);
            Actions.Add(simpleAction_EditDataSource);
        }

        

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction simpleAction_AddDataSource;
        private DevExpress.ExpressApp.Actions.SimpleAction simpleAction_EditDataSource;
    }
}
