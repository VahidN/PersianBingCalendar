using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace PersianBingCalendar.Models;

/// <remarks />
[GeneratedCode(tool: "xsd", version: "2.0.50727.3038")]
[SerializableAttribute]
[DebuggerStepThrough]
[DesignerCategory(category: "code")]
[XmlType(AnonymousType = true)]
[XmlRoot(Namespace = "", IsNullable = false)]
public class images
{
    private imagesImage? imageField;

    private imagesTooltips? tooltipsField;

    /// <remarks />
    public imagesImage? image
    {
        get => imageField;
        set => imageField = value;
    }

    /// <remarks />
    public imagesTooltips? tooltips
    {
        get => tooltipsField;
        set => tooltipsField = value;
    }
}
