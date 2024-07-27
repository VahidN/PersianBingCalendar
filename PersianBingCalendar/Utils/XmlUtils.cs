using System.IO;
using System.Xml.Serialization;

namespace PersianBingCalendar.Utils;

public static class XmlUtils
{
	public static T FromXmlFile<T>(this string filePath)
	{
		string allText = File.ReadAllText(path: filePath);
		return FromXmlContent<T>(allText: allText);
	}

	public static T FromXmlContent<T>(this string allText)
	{
		using StringReader stringReader  = new(s: allText);
		XmlSerializer      xmlSerializer = new(type: typeof(T));
		return (T)xmlSerializer.Deserialize(textReader: stringReader);
	}
}