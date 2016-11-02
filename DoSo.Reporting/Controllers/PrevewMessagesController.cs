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
using DevExpress.ExpressApp.DC;
using System.ComponentModel;
using DoSo.Reporting.BusinessObjects.Base;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Xpo;
using DevExpress.ExpressApp.Win.SystemModule;

namespace DoSo.Reporting.Controllers
{
    public partial class PrevewMessagesController : ObjectViewController<DetailView, DoSoScheduleBase>
    {
        public PrevewMessagesController()
        {
            InitializeComponent();
        }
        protected override void OnActivated()
        {
            base.OnActivated();


            this.popupWindowShowAction_PrevewMessages.CustomizePopupWindowParams += PopupWindowShowAction_PrevewMessages_CustomizePopupWindowParams;
            popupWindowShowAction_PrevewMessages.CustomizeTemplate += PopupWindowShowAction_PrevewMessages_CustomizeTemplate;
            // Perform various tasks depending on the target View.
        }


        private void SimpleAction_PrevewMessages_Execute(object sender, DevExpress.ExpressApp.Actions.SimpleActionExecuteEventArgs e)
        {
            var os = Application.CreateObjectSpace() as XPObjectSpace;
            var list = ViewCurrentObject.GenerateMessages(os.Session, true);
            if (!list.Any())
                return;

            var type = list.FirstOrDefault().GetType();

            var collection = new CollectionSource(os, type);
            var listView = Application.CreateListView(Application.FindListViewId(type), collection, false);
            listView.CreateControls();

            var itemInOs = os.GetCollectionSorting(list);// .GetObjects(typeof(DoSoMessageBase), CriteriaOperator.Parse("ID = 0"), true);

            foreach (var item in list)
                listView.CollectionSource.List.Add(item);
            //e.Context = TemplateContext.LookupControl;

            var svp = new ShowViewParameters
            {
                CreatedView = listView,
                NewWindowTarget = NewWindowTarget.Default,
                TargetWindow = TargetWindow.NewModalWindow,
                Context = TemplateContext.View
            };
            svp.Controllers.Add(new ToolbarVisibilityController());
            Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(null, null));
        }


        private void PopupWindowShowAction_PrevewMessages_CustomizeTemplate(object sender, CustomizeTemplateEventArgs e)
        {
            
        }

        private void PopupWindowShowAction_PrevewMessages_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var os = Application.CreateObjectSpace() as XPObjectSpace;
            var list = ViewCurrentObject.GenerateMessages(os.Session, true);
            if (!list.Any())
                return;

            var type = list.FirstOrDefault().GetType();

            var collection = new CollectionSource(os, type);
            var listView = Application.CreateListView(Application.FindListViewId(type), collection, false);
            listView.CreateControls();

            var itemInOs = os.GetCollectionSorting(list);// .GetObjects(typeof(DoSoMessageBase), CriteriaOperator.Parse("ID = 0"), true);

            foreach (var item in list)
                listView.CollectionSource.List.Add(item);
            //e.Context = TemplateContext.LookupControl;
            
            e.DialogController.SaveOnAccept = false;
            DialogController dc = Application.CreateController<DialogController>();
            
            e.View = listView;
        }

        private void PopupWindowShowAction_PrevewMessages_Execute(object sender, DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventArgs e)
        {

        }
    }


    [DomainComponent]
    public class ShowMessageHelper
    {
        public ShowMessageHelper() { }
        public BindingList<DoSoMessageBase> Smses;
    }
}
