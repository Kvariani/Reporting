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
using DoSo.Reporting.BusinessObjects;
using DevExpress.XtraSpreadsheet;
using System.IO;
using DevExpress.Spreadsheet;
using DevExpress.XtraLayout;
using System.Windows.Forms;
using DevExpress.ExpressApp.Win.Editors;


namespace DoSo.Reporting.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class ReportExecutionViewController : ObjectViewController<DetailView, ReportExecution>
    {
        public ReportExecutionViewController()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();

            ObjectSpace.ObjectChanged += ObjectSpace_ObjectChanged;
            // Perform various tasks depending on the target View.
        }

        private void ObjectSpace_ObjectChanged(object sender, ObjectChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ReportExecution.DoSoReport))
            {
                var layoutControl = View.Control as LayoutControl;
                var group = layoutControl.Items.OfType<LayoutControlGroup>().FirstOrDefault(x => x.CustomizationFormText == "Parameters");
                //View.CreateControls();

                var items = group.Items;
                //group.Dispose();
                while (group.Items.Any())
                {
                    var item = group.Items.FirstOrDefault() as LayoutControlItem;
                    if (item != null)
                    {
                        item.Control?.Hide();
                        item.Control?.Dispose();
                        group.Remove(item);
                    }
                }

                ViewCurrentObject?.SpreadsheetControl?.Dispose();

                if (ViewCurrentObject.DoSoReport != null)
                {
                    var report = ViewCurrentObject.DoSoReport;
                    var xml = report.Xml;
                    ViewCurrentObject.SpreadsheetControl = new SpreadsheetControl();
                    using (var ms = new MemoryStream(Convert.FromBase64String(xml)))
                        ViewCurrentObject.SpreadsheetControl.LoadDocument(ms, DocumentFormat.OpenXml);

                    var ds = ViewCurrentObject.SpreadsheetControl.Document.MailMergeDataSource as DevExpress.DataAccess.Sql.SqlDataSource;
                    if (ds == null)
                        return;
                    
                    var parameters = ds.Queries.SelectMany(x => x.Parameters);


                    foreach (var parameter in parameters)
                    {
                        var item = new LayoutControlItem() { Name = parameter.Name, OptionsToolTip = new BaseLayoutItemOptionsToolTip() { ToolTip = parameter.Name } };


                        var type = parameter.Type;

                        if (type == typeof(int))
                            item.Control = new IntegerEdit() { Dock = DockStyle.Fill, EditValue = Convert.ToInt32(parameter.Value), ToolTip = parameter.Name };
                        if (type == typeof(string))
                            item.Control = new StringEdit() { Dock = DockStyle.Fill, EditValue = Convert.ToInt32(parameter.Value), ToolTip = parameter.Name };
                        if (type == typeof(decimal))
                            item.Control = new DecimalEdit() { Dock = DockStyle.Fill, EditValue = Convert.ToInt32(parameter.Value), ToolTip = parameter.Name };
                        if (type == typeof(DateTime))
                            item.Control = new DateTimeEdit() { Dock = DockStyle.Fill, EditValue = Convert.ToInt32(parameter.Value), ToolTip = parameter.Name };

                        //item.Control = new StringEdit(250) { Dock = DockStyle.Fill, EditValue = parameter.Value, ToolTip = parameter.Name }; break;
                        (item.Control as DevExpress.XtraEditors.BaseEdit).EditValueChanged += (ss, ee) =>
                        {
                            var value = (ss as DevExpress.XtraEditors.BaseEdit).EditValue;
                            if (ss is IntegerEdit)
                                parameter.Value = Convert.ToInt32(value);
                            if (ss is StringEdit)
                                parameter.Value = value.ToString();
                            if (ss is DecimalEdit)
                                parameter.Value = Convert.ToDecimal(value);
                            if (ss is DateTimeEdit)
                                parameter.Value = Convert.ToDateTime(value);


                        };// ReportExecutionViewController_EditValueChanged(ss, ee, parameter as QueryParameter);

                        group.AddItem(item);
                    }
                }
                var defaultItem = new EmptySpaceItem();
                group.AddItem(defaultItem);
            }
        }

        private void Item_TextChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
    }
}
