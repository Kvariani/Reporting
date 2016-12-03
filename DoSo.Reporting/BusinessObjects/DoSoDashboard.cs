using NewBaseModule.BisinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpo;
using DevExpress.Persistent.Base;
using System.Xml;
using System.IO;
using DevExpress.DashboardWin;
using DevExpress.ExpressApp.Xpo;
using DevExpress.ExpressApp.Model;
using DevExpress.Xpo.Metadata;
using System.Drawing;
using DevExpress.DashboardCommon;

namespace DoSo.Reporting.BusinessObjects
{
    [DefaultClassOptions]
    public class DoSoDashboard : NewXPLiteObjectEx
    {
        public DoSoDashboard(Session session) : base(session) { }

        private string fName;
        public string Name
        {
            get { return fName; }
            set { SetPropertyValue(nameof(Name), ref fName, value); }
        }

        private int fIndex;
        public int Index
        {
            get { return fIndex; }
            set { SetPropertyValue(nameof(Index), ref fIndex, value); }
        }

        private bool fVisibleInNavigation;
        public bool VisibleInNavigation
        {
            get { return fVisibleInNavigation; }
            set { SetPropertyValue(nameof(VisibleInNavigation), ref fVisibleInNavigation, value); }
        }

        private string fXml;
        [Size(SizeAttribute.Unlimited)]
        public string Xml
        {
            get { return fXml; }
            set { SetPropertyValue(nameof(Xml), ref fXml, value); }
        }

        [ModelDefault("DetailViewImageEditorFixedHeight", "32"), ModelDefault("DetailViewImageEditorFixedWidth", "32"), ModelDefault("ListViewImageEditorCustomHeight", "32")]
        [ImmediatePostData, ValueConverter(typeof(ImageValueConverter))]
        [Size(SizeAttribute.Unlimited), Delayed(true)]
        public Image Icon
        {
            get { return GetDelayedPropertyValue<Image>("Icon"); }
            set { SetDelayedPropertyValue("Icon", value); }
        }


        public void LoadDashboardDesignerFromXml(DashboardDesignerForm form)
        {
            if (string.IsNullOrWhiteSpace(Xml))
                CreateNewDashboard(form);
            else
                using (var ms = new MemoryStream())
                {
                    using (var sr = new StreamWriter(ms, Encoding.Default))
                    {
                        var doc = new XmlDocument();
                        doc.LoadXml(Xml);
                        var definitionXml = doc.OuterXml;
                        sr.Write(definitionXml);
                        sr.Flush();
                        ms.Position = 0;
                        CreateNewDashboard(form);
                        form.dashboardDesigner1.LoadDashboard(ms);
                    }
                }
        }


        public Dashboard CreateDashBoard()
        {
            var dashboard = new Dashboard();
            LoadFromXml(Xml, dashboard);
            return dashboard;
        }

        static void LoadFromXml(string xml, Dashboard dashboard)
        {
            if (xml != null)
            {
                using (var me = new MemoryStream())
                {
                    var sw = new StreamWriter(me);
                    sw.Write(xml);
                    sw.Flush();
                    me.Seek(0, SeekOrigin.Begin);
                    dashboard.LoadFromXml(me);
                    sw.Close();
                    me.Close();
                }
            }
        }

        void CreateNewDashboard(DashboardDesignerForm form)
        {
            form.dashboardDesigner1.DashboardSaving += DashboardDesigner1_DashboardSaving;
            form.dashboardDesigner1.DataSourceWizardSettings.EnableCustomSql = true;
            form.dashboardDesigner1.DataSourceWizardSettings.AvailableDataSourceTypes = DashboardDesignerDataSourceType.Sql | DashboardDesignerDataSourceType.Excel;
            form.dashboardDesigner1.DataSourceWizardSettings.ShowConnectionsFromAppConfig = true;
            form.editExtractSourceConnectionBarItem1.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
        }

        private void DashboardDesigner1_DashboardSaving(object sender, DashboardSavingEventArgs e)
        {
            e.Handled = true;
            using (var ms = new MemoryStream())
            {
                e.Dashboard.SaveToXml(ms);
                ms.Position = 0;
                using (var sr = new StreamReader(ms, Encoding.Default))
                {
                    var xml = sr.ReadToEnd();
                    Xml = xml;
                    XPObjectSpace.FindObjectSpaceByObject(this)?.CommitChanges();
                }
            }
        }
    }
}
