using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using System;


namespace DoSo.Reporting.DatabaseUpdate
{
    public class Updater : ModuleUpdater
    {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) { }
    }
}