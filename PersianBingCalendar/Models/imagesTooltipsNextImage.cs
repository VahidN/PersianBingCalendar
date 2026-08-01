using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PersianBingCalendar.Models;

/// <remarks />
[GeneratedCode(tool: "xsd", version: "2.0.50727.3038")]
[Serializable]
[DebuggerStepThrough]
[DesignerCategory(category: "code")]
[XmlType(AnonymousType = true)]
public class imagesTooltipsNextImage
{
    private string? textField;

    /// <remarks />
    public string? text
    {
        get => textField;
        set => textField = value;
    }
}