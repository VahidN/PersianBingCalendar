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
public class imagesTooltips
{
    private imagesTooltipsLoadMessage? loadMessageField;

    private imagesTooltipsNextImage? nextImageField;

    private imagesTooltipsPause? pauseField;

    private imagesTooltipsPlay? playField;

    private imagesTooltipsPreviousImage? previousImageField;

    /// <remarks />
    public imagesTooltipsLoadMessage? loadMessage
    {
        get => loadMessageField;
        set => loadMessageField = value;
    }

    /// <remarks />
    public imagesTooltipsPreviousImage? previousImage
    {
        get => previousImageField;
        set => previousImageField = value;
    }

    /// <remarks />
    public imagesTooltipsNextImage? nextImage
    {
        get => nextImageField;
        set => nextImageField = value;
    }

    /// <remarks />
    public imagesTooltipsPlay? play
    {
        get => playField;
        set => playField = value;
    }

    /// <remarks />
    public imagesTooltipsPause? pause
    {
        get => pauseField;
        set => pauseField = value;
    }
}