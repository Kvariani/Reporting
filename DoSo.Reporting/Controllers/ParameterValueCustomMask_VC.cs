using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid.Views.Grid;
using DoSo.Reporting.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using ListView = DevExpress.ExpressApp.ListView;

namespace DoSo.Reporting.Controllers
{
    public partial class ParameterValueCustomMask_VC : ObjectViewController<ListView, QueryParameter>
    {
        public ParameterValueCustomMask_VC()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetViewNesting = Nesting.Nested;
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            _gridListEditor = View.Editor as GridListEditor;
            View.CollectionSource.CollectionChanged += CollectionSource_CollectionChanged;
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            _gridListEditor.GridView.CustomRowCellEdit += GridView_CustomRowCellEdit;
        }

        readonly Dictionary<string, DataTypeEnnum> _items = new Dictionary<string, DataTypeEnnum>();

        void CollectionSource_CollectionChanged(object sender, EventArgs e)
        {
            var list = View.CollectionSource.List;
            _items.Clear();
            if (list == null) return;

            foreach (var item in list)
            {
                var singleItem = item as QueryParameter;
                if (singleItem != null)
                    _items.Add(singleItem.ParameterName, singleItem.DataType);
            }
        }

        GridListEditor _gridListEditor;

        private void GridView_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            if (e.RowHandle < 0) return;
            var view = sender as GridView;

            if (view == null)
                return;

            if (e.Column.FieldName != nameof(QueryParameter.ParameterValue)) return;
            var parameter = view.GetRowCellValue(e.RowHandle, nameof(QueryParameter.ParameterName)).ToString().ToLower();
            var type = _items.FirstOrDefault(x => x.Key.ToString().ToLower() == parameter).Value;

            switch (type)
            {
                case DataTypeEnnum.Enum:
                    var currentParameter = View.CollectionSource.List.OfType<QueryParameter>().FirstOrDefault(x => x.ParameterName == parameter);
                    var enumTypeString = currentParameter?.EnumType;
                    if (enumTypeString != null)
                    {
                        var enumType = Type.GetType(enumTypeString.Name);
                        var enumEditor = new RepositoryItemEnumEdit(enumType);
                        e.RepositoryItem = enumEditor;
                    }
                    break;
                case DataTypeEnnum.Datetime:
                    var dateEditor = new RepositoryItemDateTimeEdit(HS.ReportCustomDateTimeFormat, HS.ReportCustomDateTimeFormat);
                    e.RepositoryItem = dateEditor;
                    break;
                case DataTypeEnnum.Text:
                    e.RepositoryItem = new RepositoryItemStringEdit();
                    break;
                case DataTypeEnnum.Integer:
                    e.RepositoryItem = new RepositoryItemIntegerEdit("", "{d:0}");
                    break;
                case DataTypeEnnum.BusinessObject:
                    var businessObject = (View.CollectionSource.List[e.RowHandle] as QueryParameter)?.BusinessObjectType;
                    if (businessObject == null) return;
                    IModelMemberViewItem targetType = View.Model.Columns[nameof(QueryParameter.BusinessObjectType)];
                    var lookupEditorHelper = new LookupEditorHelper(Application, ObjectSpace, XafTypesInfo.Instance.FindTypeInfo(businessObject), targetType);
                    var editor = new RepositoryItemLookupEdit();
                    editor.Init(View.Items.First(), string.Empty, lookupEditorHelper);
                    e.RepositoryItem = editor;
                    break;
                default:
                    return;
            }
        }
    }
}