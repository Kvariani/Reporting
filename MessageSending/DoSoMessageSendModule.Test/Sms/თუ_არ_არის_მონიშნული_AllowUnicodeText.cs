using DoSo.Reporting.BusinessObjects.SMS;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DoSoMessageSendModule.Test
{
    [TestClass]
    public class თუ_არ_არის_მონიშნული_AllowUnicodeText : concerns
    {
        [TestMethod]
        public void მეთდომა_ChangeGeorgianText_უნდა_დააბრუნოს_ინგლისური_ტექსტი()
        {
            var schedule = new DoSoSmsSchedule(objectSpace.Session);
            schedule.AllowUnicodeText = true;

            var englishText = "abgdevztiklmnopzhrstufkghkshchcdztschkhjh+-1234567890.,;:?!@$ ludi araki ghvino chinchari khakhvi";
            //var geoText = "აბგდევზთიკლმნოპჟრსტუფქღყშჩცძწჭხჯჰ+-1234567890.,;:?!@$ ლუდი არაყი ღვინო ჭინჭარი ხახვი";
            var newText = "";

            //if (schedule.AllowUnicodeText)
            //    newText = SenderHelper.ChangeGeorgianText(geoText);

            Assert.AreEqual(englishText, newText);
        }
    }
}
