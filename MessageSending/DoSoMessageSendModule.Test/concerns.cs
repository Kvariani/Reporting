using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using Microsoft.VisualStudio.TestTools.UnitTesting;


public class TestApplication : XafApplication
{
    protected override LayoutManager CreateLayoutManagerCore(bool simple)
    {
        return null;
    }
}

namespace DoSoMessageSendModule.Test
{
    [TestClass]
    public abstract class concerns
    {
        protected TestApplication application;
        internal XPObjectSpace objectSpace;

        [TestInitialize]
        public void setup_context()
        {
            var objectSpaceProvider =
                new XPObjectSpaceProvider(new MemoryDataStoreProvider());


            application = new TestApplication();
            var testModule = new ModuleBase();
            beforeSetup(testModule);
            application.Modules.Add(testModule);

            application.DatabaseUpdateMode = DatabaseUpdateMode.UpdateDatabaseAlways;

            application.Setup("TestApplication", objectSpaceProvider);
            objectSpace = objectSpaceProvider.CreateObjectSpace() as XPObjectSpace;
            XpoDefault.DataLayer = objectSpace.Session.DataLayer;
            //Do test-specific tasks in the context method

            ImageLoader.Reset();
            if (!ImageLoader.IsInitialized)
            { ImageLoader.Init(new AssemblyResourceImageSource(GetType().Assembly.FullName, "Images1")); }

            context();
        }

        protected virtual void beforeSetup(ModuleBase testModule) { }

        protected virtual void context() { }

        protected virtual void decontext() { }

        [TestCleanup]
        public void cleanup_context()
        {
            decontext();
        }
    }
}