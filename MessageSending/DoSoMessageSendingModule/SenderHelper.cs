using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DoSoMessageSendService
{
    public static class SenderHelper
    {
        public static List<Assembly> assemblies;
        private static IDataStore _Connection;
        public static string serviceDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;

        static Type GetMyObjectType(string objectName)
        {
            var assembly = assemblies.FirstOrDefault(x => x.GetType(objectName) != null);
            if (assembly != null)
                return assembly.GetType(objectName);

            return null;
        }

        public static string ChangeGeorgianText(string inputText)
        {
            const string latinText = "abgdevztiklmnoprstufkkcjh";
            const string geoText = "აბგდევზთიკლმნოპრსტუფქყცჯჰ";

            if (latinText.Length != geoText.Length)
                CreateLogFileWithException("latinText.len != geoText.len");

            for (int i = 0; i < geoText.Length; i++)
                inputText = inputText.Replace(geoText[i], latinText[i]);

            inputText = inputText.Replace("შ", "sh");
            inputText = inputText.Replace("ჩ", "ch");
            inputText = inputText.Replace("ძ", "dz");
            inputText = inputText.Replace("ჭ", "ch");
            inputText = inputText.Replace("ხ", "kh");
            inputText = inputText.Replace("წ", "ts");
            inputText = inputText.Replace("ჟ", "zh");
            inputText = inputText.Replace("ღ", "gh");

            return inputText;
        }


        public static IDataStore GetConnection()
        {
            if (_Connection == null)
            {
                var connectionString = GetConnectionString();
                _Connection = XpoDefault.GetConnectionProvider(connectionString, AutoCreateOption.SchemaAlreadyExists);
            }
            return _Connection;
        }

        public static ICollection GetMyObjects(string criteria, UnitOfWork unitOfWork, string objectTypeName, bool ReturnSomeObject, int topReturnedObjects)
        {
            var type = GetMyObjectType(objectTypeName);

            if (string.IsNullOrEmpty(criteria))
                criteria = "false";

            var objects = unitOfWork.GetObjects(unitOfWork.Dictionary.GetClassInfo(type), CriteriaOperator.Parse(criteria), null, topReturnedObjects, false, true);
            if (objects.Count == 0)
                objects = unitOfWork.GetObjects(unitOfWork.Dictionary.GetClassInfo(type), CriteriaOperator.Parse("True"), null, 1, false, true);

            return objects;
        }

        static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["DB"].ToString();
        }

        public static void LoadRequiredAssemblies()
        {
            assemblies = new List<Assembly>();

            var folderName = Path.Combine(SenderHelper.serviceDirectoryPath, "RequiredAssemblies");
            if (!Directory.Exists(folderName))
                Directory.CreateDirectory(folderName);

            foreach (string file in Directory.EnumerateFiles(folderName, "*.dll"))
                assemblies.Add(Assembly.LoadFrom(file));

            if (assemblies.Count == 0)
                CreateLogFileWithException("Cannot Find RequiredAssembly");

            var referencedAssemblies = Assembly.GetExecutingAssembly().GetReferencedAssemblies().Where(y => !assemblies.Any(z => z.FullName == y.FullName));

            foreach (var item in referencedAssemblies)
                assemblies.Add(Assembly.Load(item));

            foreach (var assembly in assemblies)
            {
                AppDomain.CurrentDomain.Load(assembly.GetName());
                EnumProcessingHelper.RegisterEnums(assembly);
            }
        }

        public static void CreateLogFileWithException(string exception)
        {
            TextWriter tw = new StreamWriter(Path.Combine(SenderHelper.serviceDirectoryPath, "MessageSenderLog_" + DateTime.Now.ToString("HHmmssfff") + ".txt"), true);
            tw.WriteLine(exception);
            tw.Close();
        }
    }
}
