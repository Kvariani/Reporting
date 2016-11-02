using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DoSo.Reporting.BusinessObjects;
using DoSo.Reporting.BusinessObjects.Email;
using DoSo.Reporting.BusinessObjects.SMS;
using FastExcelExportingDemoCs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using DevExpress.Persistent.Base;

namespace DoSo.Reporting.Generators
{
    public static class GeneratorHelper
    {
        public static List<Assembly> assemblies;
        private static IDataStore _Connection;
        public static string serviceDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
        public static List<object> objectsList2GenerateMessage;

        public static Type GetMyObjectType(string objectName)
        {
            if (assemblies == null)
                assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

            var assembly = assemblies.FirstOrDefault(x => x.GetType(objectName) != null);
            var type = assembly?.GetType(objectName);
            return type;
        }

        static string ReplaceCriteria(this string criteria)
        {
            return criteria.Replace("]", "").Replace("[", "");
        }

        public static List<IFileData> TryGetFileData(UnitOfWork uow, XPBaseObject item, string criteria, int recursiveCount)
        {
            var splitedCriteria = criteria.Split(new string[] { "].[" }, StringSplitOptions.None);
            var singleCriteria = splitedCriteria[recursiveCount];
            var splitedSingleCriteria = singleCriteria.Split(new string[] { "[[" }, StringSplitOptions.None);
            var items = item.GetMemberValue(splitedSingleCriteria.FirstOrDefault().ReplaceCriteria());


            if (items is XPBaseCollection)
            {
                var list = new List<IFileData>();
                var collection = items as XPBaseCollection;
                foreach (XPBaseObject collectionItem in collection)
                {
                    if (splitedSingleCriteria.Count() == 2)
                    {
                        var classInfo = uow.GetClassInfo(collectionItem);
                        var properties = uow.GetProperties(classInfo);

                        var result = Convert.ToBoolean(new ExpressionEvaluator(properties, CriteriaOperator.Parse(splitedSingleCriteria.LastOrDefault().ReplaceCriteria())).Evaluate(collectionItem) ?? false);
                        if (!result)
                            continue;
                    }
                    list.Add(collectionItem.GetMemberValue(splitedCriteria.LastOrDefault().ReplaceCriteria()) as IFileData);

                }

                return list;
            }
            if (items is IFileData)
            {
                var list = new List<IFileData>() { items as IFileData };
                return list;
            }
            if (splitedCriteria.Count() > 1)
                TryGetFileData(uow, items as XPBaseObject, criteria, recursiveCount + 1);
            return null;
        }

        public static DoSoEmail GenerateEmail(UnitOfWork unitOfWork, object item, string messageSubject, string to, string cc, string messageBody, DoSoReportSchedule schedule = null)
        {
            var classInfo = unitOfWork.GetClassInfo(item);
            var properties = unitOfWork.GetProperties(classInfo);
            var mailTo = new ExpressionEvaluator(properties, to).Evaluate(item)?.ToString();


            if (string.IsNullOrEmpty(mailTo))
                return null;

            var subject = new ExpressionEvaluator(properties, messageSubject).Evaluate(item)?.ToString();
            var body = new ExpressionEvaluator(properties, messageBody).Evaluate(item)?.ToString();

            var key = classInfo.KeyProperty.GetValue(item).ToString();

            var sameMailFromDb = unitOfWork.Query<DoSoEmail>().Where(x =>
                                                                x.ExpiredOn == null &&
                                                                x.EmailTo == mailTo &&
                                                                x.ObjectKey == key &&
                                                                x.ObjectTypeName == item.GetType().FullName).ToList();

            var oldNotSentMail = sameMailFromDb.Where(x => !x.IsSent && !x.IsCanceled && x.SendingDate < DateTime.Now && x.DoSoReportSchedule == schedule);
            foreach (var oldMail in oldNotSentMail)
            {
                oldMail.IsCanceled = true;
                oldMail.StatusComment = "Created New Email";
            }

            //var alredySentMails = sameMailFromDb.FirstOrDefault(x =>
            //                                        x.IsSent &&
            //                                        !x.IsCanceled &&
            //                                        x.SentDate.AddDays(schedule.SkipExecutionDate) > DateTime.Now);


            var email = new DoSoEmail(unitOfWork)
            {
                EmailTo = mailTo,
                EmailSubject = subject,
                EmailCC = cc,
                EmailBody = body,
                SendingDate = DateTime.Now,
                ObjectKey = key,
                ObjectTypeName = item.GetType().FullName,
                //SourceFilePath = path + fullName,
                DoSoReportSchedule = schedule
            };

            var folderName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GeneratedFiles");

            if (schedule?.ReportDefinition != null)
            {
                //var path = System.IO.Path.GetTempPath();

                if (!Directory.Exists(folderName))
                    Directory.CreateDirectory(folderName);

                var name = DateTime.Now.ToString("MMMddHHmmssfff");
                var fullName = name + ".xlsx";

                SetParameteValues(schedule, item, unitOfWork);

                //var providerType = (unitOfWork.DataLayer as SimpleDataLayer)?.ConnectionProvider;

                var exported = false;
                //if (providerType is MSSqlConnectionProvider)
                //exported = FastExportingMethod.ExportData2Excel(schedule.ReportDefinition, /*unitOfWork.Connection as SqlConnection*/unitOfWork, folderName, name);
                //else
                //    exported = FastExportingMethod.ExportData2ExcelFromPostgre(schedule.ReportDefinition, unitOfWork.Connection as SqlConnection, folderName, name);

                if (exported)
                    email.SourceFilePath += Path.Combine(folderName, fullName) + ";";
                else
                    email.EmailBody += string.Format("{0}{0}{1}", Environment.NewLine, schedule.AlternativeText);
            }


            if (!string.IsNullOrEmpty(schedule?.AttachedFilesExpression))
            {
                try
                {
                    var data = TryGetFileData(unitOfWork, item as XPBaseObject, schedule.AttachedFilesExpression.Replace("'", ""), 0);
                    if (data != null)
                    {

                        if (!Directory.Exists(folderName))
                            Directory.CreateDirectory(folderName);

                        foreach (var fileData in data)
                        {
                            var extention = Path.GetExtension(fileData.FileName);
                            var name = DateTime.Now.ToString("ddHHmmssfff");
                            var newFileName = $"{fileData.FileName.Replace(extention, "")}_{name}{extention}";
                            var fullPath = Path.Combine(folderName, newFileName);
                            using (var stream = new FileStream(fullPath, FileMode.Create))
                            {
                                fileData.SaveToStream(stream);
                                email.SourceFilePath += fullPath + ";";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    CreateLogFileWithException(ex.ToString());
                }
            }


            return email;
        }

        static void SetParameteValues(DoSoReportSchedule schedule, object item, UnitOfWork unitOfWork)
        {
            foreach (var parameter in schedule.QueryParametersCollection.Where(x => x.ExpiredOn == null))
            {
                var parameterFromReport = schedule.ReportDefinition.QueryParametersCollection.Where(x => x.ExpiredOn == null && x.ParameterName == parameter.ParameterName);
                var parameterValue = new ExpressionEvaluator(unitOfWork.GetProperties(unitOfWork.GetClassInfo(item)), parameter.ParameterValueExression).Evaluate(item);
                parameterFromReport.FirstOrDefault().ParameterValue = parameterValue?.ToString();
            }
        }

        public static DoSoSms GenerateSms(UnitOfWork unitOfWork, object item, string to, string text, bool allowUnicodeText, int skipExecutionDate, DoSoSmsSchedule schedule = null)
        {
            var classInfo = unitOfWork.GetClassInfo(item);
            var properties = unitOfWork.GetProperties(classInfo);

            var key = classInfo?.KeyProperty?.GetValue(item);

            var obj = unitOfWork.GetObjectByKey(item.GetType(), key);

            var smsTo = new ExpressionEvaluator(properties, to).Evaluate(obj)?.ToString();
            if (string.IsNullOrEmpty(smsTo) || smsTo.Length < 3)
                return null;

            var smsText = new ExpressionEvaluator(properties, text).Evaluate(obj)?.ToString();
            if (!allowUnicodeText)
                smsText = GeneratorHelper.ChangeGeorgianText(smsText);


            var objectTypeName = obj?.GetType()?.FullName;

            var normalizedSmsTo = smsTo?.Trim()?.Replace(" ", "").Replace("-", "").Replace("_", "");

            var sameSmsFromDb = unitOfWork.Query<DoSoSms>().Where(x =>
                                                                x.SmsTo == normalizedSmsTo &&
                                                                x.ExpiredOn == null &&
                                                                x.ObjectKey == key.ToString() &&
                                                                x.ObjectTypeName == objectTypeName).ToList();

            var oldNotSentSms = sameSmsFromDb.Where(x => !x.IsSent && !x.IsCanceled && x.SendingDate < DateTime.Now && x.DoSoSmsSchedule == schedule);

            foreach (var oldSms in oldNotSentSms)
            {
                oldSms.IsCanceled = true;
                oldSms.StatusComment = "შეიქმნა ახალი SMS";
            }

            var alredySentSms = sameSmsFromDb.FirstOrDefault(x =>
                                                                x.IsSent &&
                                                                !x.IsCanceled &&
                                                                x.SentDate.AddDays(skipExecutionDate) > DateTime.Now);

            if (alredySentSms != null)
                return null;

            var sms = new DoSoSms(unitOfWork)
            {
                SmsTo = normalizedSmsTo,
                SmsText = smsText,
                ObjectKey = key.ToString(),
                ObjectTypeName = objectTypeName,
                DoSoSmsSchedule = schedule,
                SendingDate = DateTime.Now
            };

            if (!Regex.IsMatch(sms.SmsTo, @"\d"))
            {
                sms.IsCanceled = true;
                sms.StatusComment = "ტელეფონის ნომერი არასწორ ფორმატშია";
            }

            return sms;
        }

        public static string ChangeGeorgianText(string inputText)
        {
            const string latinText = "abgdevztiklmnoprstufkkcjh";
            const string geoText = "აბგდევზთიკლმნოპრსტუფქყცჯჰ";

            if (latinText.Length != geoText.Length)
                CreateLogFileWithException("latinText.len != geoText.len");

            for (var i = 0; i < geoText.Length; i++)
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


        //public static IDataStore GetConnection()
        //{
        //    if (_Connection == null)
        //    {
        //        var connectionString = GetConnectionString();
        //        _Connection = XpoDefault.GetConnectionProvider(connectionString, AutoCreateOption.SchemaAlreadyExists);
        //    }
        //    return _Connection;
        //}

        //public static ICollection GetMyObjects(string criteria, UnitOfWork unitOfWork, string objectTypeName, bool ReturnSomeObject, int topReturnedObjects)
        //{
        //    var type = GetMyObjectType(objectTypeName);

        //    if (string.IsNullOrEmpty(criteria))
        //        criteria = "false";

        //    var objects = unitOfWork.GetObjects(unitOfWork.Dictionary.GetClassInfo(type), CriteriaOperator.Parse(criteria), null, topReturnedObjects, false, true);
        //    //if (objects.Count == 0)
        //    //    objects = unitOfWork.GetObjects(unitOfWork.Dictionary.GetClassInfo(type), CriteriaOperator.Parse("True"), null, 1, false, true);

        //    return objects;
        //}

        //public static string GetConnectionString()
        //{
        //    return ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
        //}

        //public static void LoadRequiredAssemblies()
        //{
        //    assemblies = new List<Assembly>();

        //    var folderName = Path.Combine(serviceDirectoryPath, "RequiredAssemblies");
        //    if (!Directory.Exists(folderName))
        //        Directory.CreateDirectory(folderName);

        //    foreach (var file in Directory.EnumerateFiles(folderName, "*.dll"))
        //        assemblies.Add(Assembly.LoadFrom(file));

        //    if (assemblies.Count == 0)
        //        CreateLogFileWithException("Cannot Find RequiredAssembly");

        //    var referencedAssemblies = Assembly.GetExecutingAssembly().GetReferencedAssemblies().Where(y => !assemblies.Any(z => z.FullName == y.FullName));

        //    assemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies().ToList());

        //    foreach (var item in referencedAssemblies)
        //        assemblies.Add(Assembly.Load(item));

        //    foreach (var assembly in assemblies)
        //    {
        //        AppDomain.CurrentDomain.Load(assembly.GetName());
        //        EnumProcessingHelper.RegisterEnums(assembly);
        //    }
        //}

        //public static void CreateLogFileWithException(string exception)
        //{
        //    try
        //    {
        //        using (TextWriter tw = new StreamWriter(Path.Combine(serviceDirectoryPath, "ErrrorLog_" + DateTime.Now.ToString("ddHHmmssfff") + ".txt"), true))
        //        {
        //            tw.WriteLine(exception);
        //            tw.Close();
        //        }
        //    }
        //    catch (Exception) {/*Ignored*/}
        //}
    }
}
