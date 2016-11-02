using DevExpress.XtraEditors.Design;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.Design;

namespace DoSo.Reporting.Editors
{
    public class MyUnboundColumnExpressionEditorForm : UnboundColumnExpressionEditorForm
    {
        public MyUnboundColumnExpressionEditorForm(object contextInstance, IDesignerHost designerHost) : base(contextInstance, designerHost) { }
        protected override void FillFieldsTable(Dictionary<string, string> itemsTable)
        {
            base.FillFieldsTable(itemsTable);
            itemsTable.Remove("[Oid]");
        }
    }
}