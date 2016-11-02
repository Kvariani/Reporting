using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using DoSo.Reporting.BusinessObjects.Base;
using DoSo.Reporting.BusinessObjects.Email;
using DoSo.Reporting.BusinessObjects.SMS;
using DoSo.Reporting.Generators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using DevExpress.Persistent.Base;

namespace DoSo.Reporting.Controllers
{

    public partial class CreateMessageByRuleController : ViewController
    {
        public CreateMessageByRuleController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            ObjectSpace.Committing += ObjectSpace_Committing;
            ObjectSpace.Committed += ObjectSpace_Committed;
        }

        IQueryable<MessageGeneratorRule> generatorRules;

        void ObjectSpace_Committing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var objSpace = (sender as XPObjectSpace);
            generatorRules = (ObjectSpace as XPObjectSpace).Session.Query<MessageGeneratorRule>().Where(x =>
                                                                                   x.ExpiredOn == null &&
                                                                                   x.IsActive &&
                                                                                   x.BusinessObjectFullName == View.ObjectTypeInfo.FullName);

            if (generatorRules.Any())
            {
                if (GeneratorHelper.objectsList2GenerateMessage == null)
                    GeneratorHelper.objectsList2GenerateMessage = new List<object>();

                var modifiedObjects = objSpace.ModifiedObjects.OfType<object>().Where(x => generatorRules.Any(a => a.BusinessObjectFullName == x.GetType().FullName));

                foreach (var item in modifiedObjects)
                    if (GeneratorHelper.objectsList2GenerateMessage.All(x => x != item))
                        GeneratorHelper.objectsList2GenerateMessage.Add(item);
            }
        }

        string GetMessageTo(UnitOfWork unitOfWork, MessageGeneratorRule rule)
        {
            var criteria = rule.CustomMessage2ObjectCriteria;
            if (!string.IsNullOrEmpty(criteria))
            {
                var to = "";
                var objects = unitOfWork.GetObjects(unitOfWork.Dictionary.GetClassInfo(rule.BusinessObject4MessageTo), CriteriaOperator.Parse(rule.CustomMessage2ObjectCriteria), null, 100, false, true);

                foreach (var item in objects)
                    to += EvaluetedObject(unitOfWork, item, rule.CustomMessageTo) + "; ";

                return to;
            }
            return rule.MessageTo;
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork))
                return ip.ToString();
            return "";
        }

        void ObjectSpace_Committed(object sender, EventArgs e)
        {
            if (generatorRules == null || !generatorRules.Any())
                return;

            try
            {
                //var objectSpace = (sender as XPObjectSpace);
                //var session = objectSpace.Session;

                //using (var unitOfWork = new UnitOfWork(session.DataLayer))
                //{
                //    var messages = new List<DoSoMessageBase>();
                //    foreach (var item in GeneratorHelper.objectsList2GenerateMessage)
                //    {
                //        var itemInCurrentSession = objectSpace.GetObject(item);

                //        var rules = generatorRules.Where(x => x.BusinessObjectFullName == itemInCurrentSession.GetType().FullName);
                //        foreach (var rule in rules)
                //        {
                //            if (!string.IsNullOrEmpty(rule.CancellationCriteria))
                //                if (CancelNotSentMessage(unitOfWork, itemInCurrentSession, rule))
                //                    continue;

                //            if (!Convert.ToBoolean(ObjectSpace.IsObjectFitForCriteria(item, CriteriaOperator.Parse(rule.SendingCriteria))))
                //                continue;

                //            var messageTo = GetMessageTo(unitOfWork, rule);

                //            if (rule is DoSoEmailGeneratorRule)
                //            {
                //                var _rule = rule as DoSoEmailGeneratorRule;
                //                var message = GeneratorHelper.GenerateEmail(unitOfWork, itemInCurrentSession, _rule.MessageSubject, messageTo, _rule.MessageCC, _rule.MessageText);
                //                if (message != null)
                //                {
                //                    if (_rule.AttachedFileProperty != null)
                //                    {
                //                        var file =
                //                            (itemInCurrentSession as XPBaseObject)?.GetMemberValue(_rule.AttachedFileProperty.MemberName) as IFileData;
                //                        if (file != null)
                //                        {
                //                            var sharedFolderPath = ConfigurationStatic.GetParameterValue("SharedFolderPath", unitOfWork) ?? AppDomain.CurrentDomain.BaseDirectory;
                //                            var folderName = Path.Combine(sharedFolderPath, "GeneratedFiles");
                //                            if (!string.IsNullOrEmpty(_rule.GeneratedFilePath))
                //                                folderName = _rule.GeneratedFilePath;

                //                            if (!Directory.Exists(folderName))
                //                                Directory.CreateDirectory(folderName);

                //                            var fileName = DateTime.Now.ToString("MMMddHHmmssfff") + file.FileName;
                //                            var fullName = Path.Combine(folderName, fileName);
                //                            using (var stream = new FileStream(fullName, FileMode.Create))
                //                            {
                //                                file.SaveToStream(stream);
                //                                message.SourceFilePath = fullName;
                //                            }
                //                        }
                //                    }
                //                    message.ReportData = _rule.ReportData;
                //                    message.ExportFileFormat = _rule.ExportFileFormat;
                //                    messages.Add(message);
                //                }
                //            }
                //            if (rule is DoSoSmsGeneratorRule)
                //            {
                //                if (!string.IsNullOrEmpty(messageTo))
                //                    foreach (var to in messageTo.Split(';'))
                //                    {
                //                        var message = GeneratorHelper.GenerateSms(unitOfWork, itemInCurrentSession, to, rule.MessageText, false, 0);
                //                        if (message != null)
                //                            messages.Add(message);
                //                    }
                //            }

                //            if (!string.IsNullOrEmpty(rule.SendingTime) && messages.Count > 0)
                //            {
                //                var sendingDate = EvaluetedObject(unitOfWork, itemInCurrentSession, rule.SendingTime);
                //                if (sendingDate != null)
                //                    foreach (var msg in messages)
                //                        msg.SendingDate = Convert.ToDateTime(sendingDate);
                //            }

                //            messages.Clear();
                //        }
                //        unitOfWork.CommitChanges();
                //    }
                //}
            }/// ეს დროებით, მერე უნდა დავლოგო
            catch (Exception ex)
            {
                XtraMessageBox.Show(String.Format("მოხდა შეცდომა შეტყობინების გენერირებისას:{0}{1}", Environment.NewLine, ex));
            }
            GeneratorHelper.objectsList2GenerateMessage.Clear();
        }

        object EvaluetedObject(UnitOfWork unitOfWork, object item, string value)
        {
            var classInfo = unitOfWork.GetClassInfo(item);
            var properties = unitOfWork.GetProperties(classInfo);
            return new ExpressionEvaluator(properties, value).Evaluate(item);
        }

        bool CancelNotSentMessage(UnitOfWork unitOfWork, object obj, MessageGeneratorRule rule)
        {
            var cancellationCriteriaResult = ObjectSpace.IsObjectFitForCriteria(obj, CriteriaOperator.Parse(rule.CancellationCriteria));
            if (cancellationCriteriaResult != true)
                return false;

            var classInfo = unitOfWork.GetClassInfo(obj);
            var key = classInfo.KeyProperty.GetValue(obj).ToString();
            var notSentMessage = unitOfWork.Query<DoSoMessageBase>().Where(x => x.ExpiredOn == null && !x.IsSent && !x.IsCanceled && x.ObjectKey == key && x.ObjectTypeName == obj.GetType().FullName);

            foreach (var item in notSentMessage)
            {
                item.IsCanceled = true;
                item.StatusComment = string.Format("Cancelled From Rule - {0} {1}{2}{3}", rule.ID, DateTime.Now, Environment.NewLine, rule.CancellationComment);
            }

            return true;
        }


        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            ObjectSpace.Committing -= ObjectSpace_Committing;
            ObjectSpace.Committed -= ObjectSpace_Committed;
        }
    }
}
