using System.Xml;
using System.Xml.Serialization;

namespace PersianBingCalendar.Utils;

public static class XmlUtils
{
    /// <summary>
    ///     Secure XML reader settings that prevent XXE attacks (CVE-2017-9096)
    ///     by disabling DTD processing and external entity resolution.
    /// </summary>
    private static readonly XmlReaderSettings SecureXmlReaderSettings = new()
    {
        DtdProcessing = DtdProcessing.Prohibit,
        XmlResolver = null
    };

    public static T? FromXmlFile<T>(this string filePath)
    {
        var allText = File.ReadAllText(filePath);

        return allText.FromXmlContent<T>();
    }

    public static T? FromXmlContent<T>(this string allText)
    {
        using var stringReader = new StringReader(allText);
        using var reader = XmlReader.Create(stringReader, SecureXmlReaderSettings);

        var xmlSerializer = new XmlSerializer(typeof(T));

        return (T?)xmlSerializer.Deserialize(reader);
    }
}
