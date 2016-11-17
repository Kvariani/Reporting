using NewBaseModule.BisinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpo;
using DevExpress.Persistent.Base;
using DoSoReporting.Module;
using DevExpress.XtraSpreadsheet.Services;
using DoSo.Reporting.Controllers;
using DevExpress.XtraSpreadsheet;
using System.IO;
using DevExpress.Spreadsheet;

namespace DoSo.Reporting.BusinessObjects.Reporting
{
    [DefaultClassOptions]
    public class DoSoReport : NewXPLiteObjectEx
    {
        public DoSoReport(Session session) : base(session)
        {
        }

        private string fName;
        public string Name
        {
            get { return fName; }
            set { SetPropertyValue(nameof(Name), ref fName, value); }
        }

        private bool fIsActive;
        public bool IsActive
        {
            get { return fIsActive; }
            set { SetPropertyValue(nameof(IsActive), ref fIsActive, value); }
        }

        private string fXml;
        [Size(SizeAttribute.Unlimited)]
        public string Xml
        {
            get { return fXml; }
            set { SetPropertyValue(nameof(Xml), ref fXml, value); }
        }

        public static DoSoSheetFrom CreateSheetForm()
        {
            var sheetForm = new DoSoSheetFrom(true);
            
            sheetForm.spreadsheetControl1.Options.DataSourceWizard.EnableCustomSql = true;

            ISpreadsheetCommandFactoryService service = sheetForm.spreadsheetControl1.GetService(typeof(ISpreadsheetCommandFactoryService)) as ISpreadsheetCommandFactoryService;
            sheetForm.spreadsheetControl1.ReplaceService<ISpreadsheetCommandFactoryService>(new CustomCommandFactoryServise(service, sheetForm.spreadsheetControl1));

            return sheetForm;
        }

        public static string GetDocumentXml(SpreadsheetControl control)
        {
            using (var ms = new MemoryStream())
            {
                control.Document.SaveDocument(ms, DocumentFormat.OpenXml);
                ms.Position = 0;
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }
}
