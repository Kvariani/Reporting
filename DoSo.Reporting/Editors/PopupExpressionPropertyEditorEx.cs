using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Data.Browsing;
using DevExpress.Data.ExpressionEditor;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Design;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UserDesigner.Native;
using DoSo.Reporting.BusinessObjects.Base;

namespace DoSo.Reporting.Controllers
{
    [PropertyEditor(typeof(string), false)]
    public class PopupExpressionPropertyEditorEx : PopupExpressionPropertyEditor
    {
        private readonly Type _objectType;
        private readonly IModelMemberViewItem _model;
        public PopupExpressionPropertyEditorEx(Type objectType, IModelMemberViewItem model) : base(objectType, model)
        {
            CustomSetupRepositoryItem += PopupExpressionPropertyEditorEx_CustomSetupRepositoryItem;
            _objectType = objectType;
            _model = model;
        }


        private void PopupExpressionPropertyEditorEx_CustomSetupRepositoryItem(object sender, CustomSetupRepositoryItemEventArgs e)
        {
            var attribute = _objectType.GetMember(_model.PropertyName).FirstOrDefault(x => x.CustomAttributes.Any(a => a.AttributeType == typeof(ElementTypePropertyAttribute)))?.CustomAttributes.FirstOrDefault(x => x.AttributeType == typeof(ElementTypePropertyAttribute));
            var propetyName = attribute?.ConstructorArguments.FirstOrDefault();
            var editor = e.Item.OwnerEdit as PopupExpressionEditEx;
            if (editor != null)
                editor.ElementPropertyName = propetyName?.Value.ToString();
        }

        protected override RepositoryItem CreateRepositoryItem() { return new RepositoryItemPopupExpressionEditEx(_objectType, _model); }

        protected override object CreateControlCore() { return new PopupExpressionEditEx(); }
    }

    public class PopupExpressionEditEx : PopupExpressionEdit
    {
        public string ElementPropertyName;

        protected override void OnClickButton(EditorButtonObjectInfoArgs buttonInfo)
        {
            var obj = GridEditingObject as DoSoScheduleBase;
            obj.CreateDataSourceFromXml();

            if (obj.ExcelDataSource != null)
                using (var expressionEditorForm = new MyUnboundColumnExpressionEditorForm(obj.ExcelDataSource != null, new XRDesignerHost(null), EditValue?.ToString()))
                    if (expressionEditorForm.ShowDialog() == DialogResult.OK)
                        EditValue = expressionEditorForm.Expression;

            if (obj.SqlDataSource != null)
                using (var expressionEditorForm = new MyUnboundColumnExpressionEditorForm(obj.SqlDataSource.Result.FirstOrDefault(), new XRDesignerHost(null), EditValue?.ToString()))
                    if (expressionEditorForm.ShowDialog() == DialogResult.OK)
                        EditValue = expressionEditorForm.Expression;
        }

        public new RepositoryItemPopupExpressionEdit Properties => base.Properties;
    }

    public class RepositoryItemPopupExpressionEditEx : RepositoryItemPopupExpressionEdit
    {
        private readonly Type _objectType;
        private readonly IModelMemberViewItem _model;
        public RepositoryItemPopupExpressionEditEx(Type objectType, IModelMemberViewItem model)
        {
            _objectType = objectType;
            _model = model;
        }

        public override BaseEdit CreateEditor()
        {
            PopupExpressionEdit editor;
            try
            {
                editor = new PopupExpressionEditEx();
                var attribute = _objectType.GetMember(_model.PropertyName).FirstOrDefault(x => x.CustomAttributes.Any(a => a.AttributeType == typeof(ElementTypePropertyAttribute)))?.CustomAttributes.FirstOrDefault(x => x.AttributeType == typeof(ElementTypePropertyAttribute));
                var propetyName = attribute?.ConstructorArguments.FirstOrDefault();
                ((PopupExpressionEditEx)editor).ElementPropertyName = propetyName?.Value.ToString();
            }
            catch (Exception)
            {
                editor = base.CreateEditor() as PopupExpressionEdit;
            }
            return editor;
        }
    }

    public class ExpressionEditorLogicEx2 : ExpressionEditorLogicEx
    {
        public ExpressionEditorLogicEx2(IExpressionEditor editor, IDataColumnInfo columnInfo) : base(editor, columnInfo) { }

        protected override void ValidateExpressionEx(string expression)
        {
            try { base.ValidateExpressionEx(expression); }
            catch (NullReferenceException ex) {/*Ignored*/}
        }

        //protected override bool ValidateExpression()
        //{
        //    try
        //    {
        //        CriteriaOperator.Parse(ExpressionMemoEdit.Text, null);
        //        ValidateExpressionEx(ExpressionMemoEdit.Text);
        //    }
        //    catch (CriteriaParserException exception)
        //    {
        //        ShowError(exception);
        //        int verticalSelectionOffset = exception.Line * 2;
        //        for (int i = 0; i < exception.Line; i++)
        //            verticalSelectionOffset += ExpressionMemoEdit.GetLineLength(i);
        //        ExpressionMemoEdit.SelectionStart = verticalSelectionOffset + exception.Column;
        //        ExpressionMemoEdit.SelectionLength = 1;
        //        ExpressionMemoEdit.Focus();
        //        return false;
        //    }
        //    catch (Exception e)
        //    {
        //        ShowError(e);
        //        ExpressionMemoEdit.Focus();
        //        return false;
        //    }
        //    return true;
        //}
    }

    public class MyUnboundColumnExpressionEditorForm : ExpressionEditorFormEx
    {
        PopupFieldNamePicker _fieldListEditor;

        void listBox_SelectedValueChanged(object sender, EventArgs e)
        {
            var secondList = Controls.OfType<ListBoxControl>().LastOrDefault();
            var listBox = sender as ListBoxControl;
            if (listBox == null || secondList == null)
                return;

            if (listBox.SelectedValue.ToString() == "Fields")
                secondList.Controls.Add(_fieldListEditor);
            else
                secondList.Controls.Remove(_fieldListEditor);
        }

        public MyUnboundColumnExpressionEditorForm(object contextInstance, IDesignerHost designerHost/*, bool useCalculatedFields*/, string existingExpression) : base(contextInstance, designerHost)
        {
            fEditorLogic.InsertTextInExpressionMemo($"{existingExpression}");
            InitializeFieldListEditor();
            DesignLookAndFeelHelper.SetParentLookAndFeel(this, designerHost);

            SubscribeEvents();
            var listBox = Controls.OfType<ListBoxControl>().FirstOrDefault();
            if (listBox == null) return;

            listBox.Items.Add("Fields");
            listBox.SelectedValueChanged += listBox_SelectedValueChanged;
        }

        protected override ExpressionEditorLogic CreateExpressionEditorLogic()
        {
            return new ExpressionEditorLogicEx2(this, null);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            UnsubscribeEvents();
        }

        internal void ShowParametersEditor()
        {
            _fieldListEditor.Visible = true;
            _fieldListEditor.Bounds = GetParametersEditorBounds();
            _fieldListEditor.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            HideParameters();
        }

        internal void HideParametersEditor()
        {
            if (_fieldListEditor == null || !_fieldListEditor.Visible)
                return;
            _fieldListEditor.Visible = false;
            ShowParameters();
        }



        void InitializeFieldListEditor()
        {
            _fieldListEditor = new ExpressionFieldNamePicker(new DataContextOptions(true, false))
            {
                AddNoneNode = false,
                Visible = true,
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyles.Simple
            };
            _fieldListEditor.Start(ServiceProvider, ContextInstance, "", null, null);
        }

        void SubscribeEvents()
        {
            _fieldListEditor.MouseDoubleClick += fieldListEditor_MouseDoubleClick;
            _fieldListEditor.FocusedNodeChanged += fieldListEditor_FocusedNodeChanged;
        }

        void UnsubscribeEvents()
        {
            _fieldListEditor.MouseDoubleClick -= fieldListEditor_MouseDoubleClick;
            _fieldListEditor.FocusedNodeChanged -=
                fieldListEditor_FocusedNodeChanged;
        }

        void fieldListEditor_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            var node = _fieldListEditor.FocusedNode as DataMemberListNodeBase;
            if (node == null || node.Property == null)
                return;
            if (!node.HasChildren)
                SetDescription(GetResourceString("Fields Description Prefix") + node.Property.PropertyType);
            else SetDescription(string.Empty);
        }

        void fieldListEditor_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var focusedNode = _fieldListEditor.FocusedNode as DataMemberListNode;
            if (focusedNode == null)
                return;
            var propertyName = focusedNode.DataMember;
            fEditorLogic.InsertTextInExpressionMemo($"[{propertyName}]");
        }
    }

    public class ExpressionFieldNamePicker : PopupFieldNamePicker
    {
        public ExpressionFieldNamePicker(DataContextOptions options) : base(options)
        {
            OptionsBehavior.AllowExpandOnDblClick = false;
        }

        protected override bool CanCloseDropDown(DevExpress.XtraTreeList.Native.XtraListNode node)
        {
            return true;
        }
    }
}