using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using System;
using System.Collections.Generic;

namespace DoSo.Reporting
{
    public sealed partial class ReportingModule : ModuleBase
    {
        public ReportingModule()
        {
            InitializeComponent();
        }
        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB)
        {
            ModuleUpdater updater = new Reporting.DatabaseUpdate.Updater(objectSpace, versionFromDB);
            return new[] { updater };
        }
    }
}