using System.IO;
using System.Xml.Serialization;

namespace PersianBingCalendar.Utils
{
    public static class XmlUtils
    {
        public static T FromXmlFile<T>(this string filePath)
        {
            var allText = File.ReadAllText(filePath);
            return FromXmlContent<T>(allText);
        }

        public static T FromXmlContent<T>(this string allText)
        {
            using (var stringReader = new StringReader(allText))
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                return (T) xmlSerializer.Deserialize(stringReader);
            }
        }
    }
}