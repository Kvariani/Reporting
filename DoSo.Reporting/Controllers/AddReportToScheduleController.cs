using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DoSoReporting.Module;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.XtraSpreadsheet;
using DevExpress.XtraSpreadsheet.Commands;
using System.IO;
using DevExpress.Spreadsheet;
using DevExpress.XtraEditors;
using System.Windows.Forms;
using DoSo.Reporting.BusinessObjects;
using System.Xml;
using System.Runtime.InteropServices.ComTypes;
using DoSo.Reporting.BusinessObjects.Reporting;

namespace DoSo.Reporting.Controllers
{
    public partial class AddReportToScheduleController : ObjectViewController<DetailView, DoSoReportSchedule>
    {
        public AddReportToScheduleController()
        {
            InitializeComponent();
        }

        private void simpleAction_AddReportToSchedule_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var sheetForm = DoSoReport.CreateSheetForm();
            sheetForm.FormClosing += SheetForm_FormClosing;

            var xml = ViewCurrentObject?.Report?.Xml;
            if (!string.IsNullOrEmpty(xml))
                using (var ms = new MemoryStream(Convert.FromBase64String(xml)))
                    sheetForm.spreadsheetControl1.LoadDocument(ms, DocumentFormat.OpenXml);
            sheetForm.Show();
        }


        private void SpreadsheetControl1_ValidateCustomSqlQuery(object sender, SpreadsheetValidateCustomSqlQueryEventArgs e)
        {
            //e.Valid = true;
        }

        private void SheetForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var result = XtraMessageBox.Show("Do you want save changes?", "Save?", MessageBoxButtons.YesNoCancel);
            if (result == DialogResult.Yes)
            {
                var form = sender as DoSoSheetFrom;
                var xml = DoSoReport.GetDocumentXml(form.spreadsheetControl1);
                if (ViewCurrentObject.Report == null)
                    ViewCurrentObject.Report = new DoSoReport(ViewCurrentObject.Session) { Xml = xml, Name = ViewCurrentObject.ScheduleDescription ?? $"Report For Schedule - {ViewCurrentObject.ID}" };
                else
                    ViewCurrentObject.Report.Xml = xml;
                ObjectSpace.CommitChanges();
            }
            if (result == DialogResult.Cancel)
                e.Cancel = true;
        }

    }

    public class CustomCommandFactoryServise : SpreadsheetCommandFactoryServiceWrapper
    {
        SpreadsheetControl Control { get; set; }
        public CustomCommandFactoryServise(ISpreadsheetCommandFactoryService service, SpreadsheetControl _control) : base(service)
        {
            Control = _control;
        }
        SpreadsheetCommandId lastCommand;
        public override SpreadsheetCommand CreateCommand(SpreadsheetCommandId id)
        {
            //if (id == SpreadsheetCommandId.MailMergePreview)
            //{
            //    return new MailMergePreviewCommandEx(Control);
            //}

            if (id == SpreadsheetCommandId.MailMergeAddDataSource)
            {
                return new MailMergeAddDataSourceCommandEx(Control);
            }
            if (id == SpreadsheetCommandId.FileSave)
            {
                if (lastCommand == id)
                {
                    return null;
                }
                lastCommand = id;
                return new SaveCommand(Control);
            }
            lastCommand = id;
            SpreadsheetCommand command = base.CreateCommand(id);
            return command;
        }
    }


    public class MailMergePreviewCommandEx : MailMergePreviewCommand
    {
        public MailMergePreviewCommandEx(ISpreadsheetControl control) : base(control) { }

        public override void Execute()
        {
            var sqlDataSource = Control.Document.MailMergeDataSource as DevExpress.DataAccess.Sql.SqlDataSource;
            if (sqlDataSource != null)
                sqlDataSource.Fill();

            base.Execute();
        }
    }

    public class MailMergeAddDataSourceCommandEx : MailMergeAddDataSourceCommand
    {
        public MailMergeAddDataSourceCommandEx(ISpreadsheetControl control) : base(control) { }

        public override void Execute()
        {
            //var designForm = new DevExpress.XtraReports.UserDesigner.XRDesignFormEx();
            ////designForm.DesignPanel.CreateControl();
            //designForm.DesignPanel.DesignerHostLoading += DesignPanel_DesignerHostLoading;
            //designForm.DesignPanel.ExecCommand(DevExpress.XtraReports.UserDesigner.ReportCommand.NewReportWizard);
            //var ds = designForm.DesignPanel.Report.DataSource as DevExpress.DataAccess.Sql.SqlDataSource;
            //Control.Document.MailMergeDataSource = ds;

            //Control.ShowAddDataSourceForm(null);
            base.Execute();
            //var par = ds.ConnectionParameters;
            //Control.Document.MailMergeParameters.AddParameter()
        }


        private void DesignPanel_DesignerHostLoading(object sender, EventArgs e)
        {
            (sender as DevExpress.XtraReports.UserDesigner.XRDesignPanel).AddService(typeof(DevExpress.XtraReports.Wizards3.IWizardCustomizationService), new DoSoReporting.Module.Controllers.WizardCustomizationService());
        }
    }
    public class SaveCommand : SaveDocumentCommand
    {
        public SaveCommand(ISpreadsheetControl control) : base(control) { }
        public override void Execute()
        {
            // Your custom saving logic
        }
    }
}
