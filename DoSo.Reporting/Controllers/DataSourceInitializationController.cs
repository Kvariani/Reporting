using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraFilterEditor;
using DoSo.Reporting.BusinessObjects.Base;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoSo.Reporting.Controllers
{
    public class DataSourceInitializationController : ObjectViewController<DetailView, DoSoScheduleBase>
    {
        protected override void OnActivated()
        {
            base.OnActivated();

            ViewCurrentObject.Changed += ViewCurrentObject_Changed;
            ObjectSpace.Reloaded += ObjectSpace_Reloaded;
            ViewCurrentObject.CreateDataSourceFromXml();
            UpdateFieldsList(ViewCurrentObject);
        }

        private void ObjectSpace_Reloaded(object sender, EventArgs e)
        {
            ViewCurrentObject.CreateDataSourceFromXml();
            UpdateFieldsList(ViewCurrentObject);
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            ViewCurrentObject.Changed -= ViewCurrentObject_Changed;
        }

        private void ViewCurrentObject_Changed(object sender, DevExpress.Xpo.ObjectChangeEventArgs e)
        {
            if (e.PropertyName == nameof(ViewCurrentObject.DataSourceXml))
            {
                ViewCurrentObject.CreateDataSourceFromXml();
                UpdateFieldsList(ViewCurrentObject);
            }
        }

        public void UpdateFieldsList(DoSoScheduleBase schedule)
        {

            var criteriaEditors = View.GetItems<CriteriaPropertyEditor>();
            foreach (var editor in criteriaEditors)
            {
                if (editor.Control == null)
                    editor.CreateControl();

                var control = editor.Control as FilterEditorControl;
                control.FilterColumns.Clear();

                if (schedule.SqlDataSource != null)
                    foreach (var item in schedule.SqlDataSource.Result[0].Columns)
                        control.FilterColumns.Add(new FilterColumnEx(item.Name, item.Type));

                if (schedule.ExcelDataSource != null)
                    foreach (var item in schedule.ExcelDataSource.Schema)
                        control.FilterColumns.Add(new FilterColumnEx(item.Name, item.Type));

                //control.CreateControl();
            }
        }
    }
    public class FilterColumnEx : FilterColumn
    {
        string _caption;
        Type _type;
        public FilterColumnEx(string caption, Type type)
        {
            _caption = caption;
            _type = type;
        }
        public override string ColumnCaption => _caption;
        public override RepositoryItem ColumnEditor => null;
        public override Type ColumnType => _type;
        public override string FieldName => _caption;
        public override Image Image => null;
    }

}
