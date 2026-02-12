using System;
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
[XmlRoot(Namespace = "", IsNullable = false)]
public class images
{
	private imagesImage imageField;

	private imagesTooltips tooltipsField;

	/// <remarks />
	public imagesImage image
	{
		get => imageField;
		set => imageField = value;
	}

	/// <remarks />
	public imagesTooltips tooltips
	{
		get => tooltipsField;
		set => tooltipsField = value;
	}
}

/// <remarks />
[GeneratedCode(tool: "xsd", version: "2.0.50727.3038")]
[Serializable]
[DebuggerStepThrough]
[DesignerCategory(category: "code")]
[XmlType(AnonymousType = true)]
public class imagesImage
{
	private byte botField;

	private string copyrightField;

	private string copyrightlinkField;

	private byte drkField;

	private uint enddateField;

	private ulong fullstartdateField;

	private object hotspotsField;

	private uint startdateField;

	private byte topField;

	private string urlBaseField;

	private string urlField;

	/// <remarks />
	public uint startdate
	{
		get => startdateField;
		set => startdateField = value;
	}

	/// <remarks />
	public ulong fullstartdate
	{
		get => fullstartdateField;
		set => fullstartdateField = value;
	}

	/// <remarks />
	public uint enddate
	{
		get => enddateField;
		set => enddateField = value;
	}

	/// <remarks />
	public string url
	{
		get => urlField;
		set => urlField = value;
	}

	/// <remarks />
	public string urlBase
	{
		get => urlBaseField;
		set => urlBaseField = value;
	}

	/// <remarks />
	public string copyright
	{
		get => copyrightField;
		set => copyrightField = value;
	}

	/// <remarks />
	public string copyrightlink
	{
		get => copyrightlinkField;
		set => copyrightlinkField = value;
	}

	/// <remarks />
	public byte drk
	{
		get => drkField;
		set => drkField = value;
	}

	/// <remarks />
	public byte top
	{
		get => topField;
		set => topField = value;
	}

	/// <remarks />
	public byte bot
	{
		get => botField;
		set => botField = value;
	}

	/// <remarks />
	public object hotspots
	{
		get => hotspotsField;
		set => hotspotsField = value;
	}
}

/// <remarks />
[GeneratedCode(tool: "xsd", version: "2.0.50727.3038")]
[Serializable]
[DebuggerStepThrough]
[DesignerCategory(category: "code")]
[XmlType(AnonymousType = true)]
public class imagesTooltips
{
	private imagesTooltipsLoadMessage loadMessageField;

	private imagesTooltipsNextImage nextImageField;

	private imagesTooltipsPause pauseField;

	private imagesTooltipsPlay playField;

	private imagesTooltipsPreviousImage previousImageField;

	/// <remarks />
	public imagesTooltipsLoadMessage loadMessage
	{
		get => loadMessageField;
		set => loadMessageField = value;
	}

	/// <remarks />
	public imagesTooltipsPreviousImage previousImage
	{
		get => previousImageField;
		set => previousImageField = value;
	}

	/// <remarks />
	public imagesTooltipsNextImage nextImage
	{
		get => nextImageField;
		set => nextImageField = value;
	}

	/// <remarks />
	public imagesTooltipsPlay play
	{
		get => playField;
		set => playField = value;
	}

	/// <remarks />
	public imagesTooltipsPause pause
	{
		get => pauseField;
		set => pauseField = value;
	}
}

/// <remarks />
[GeneratedCode(tool: "xsd", version: "2.0.50727.3038")]
[Serializable]
[DebuggerStepThrough]
[DesignerCategory(category: "code")]
[XmlType(AnonymousType = true)]
public class imagesTooltipsLoadMessage
{
	private string messageField;

	/// <remarks />
	public string message
	{
		get => messageField;
		set => messageField = value;
	}
}

/// <remarks />
[GeneratedCode(tool: "xsd", version: "2.0.50727.3038")]
[Serializable]
[DebuggerStepThrough]
[DesignerCategory(category: "code")]
[XmlType(AnonymousType = true)]
public class imagesTooltipsPreviousImage
{
	private string textField;

	/// <remarks />
	public string text
	{
		get => textField;
		set => textField = value;
	}
}

/// <remarks />
[GeneratedCode(tool: "xsd", version: "2.0.50727.3038")]
[Serializable]
[DebuggerStepThrough]
[DesignerCategory(category: "code")]
[XmlType(AnonymousType = true)]
public class imagesTooltipsNextImage
{
	private string textField;

	/// <remarks />
	public string text
	{
		get => textField;
		set => textField = value;
	}
}

/// <remarks />
[GeneratedCode(tool: "xsd", version: "2.0.50727.3038")]
[Serializable]
[DebuggerStepThrough]
[DesignerCategory(category: "code")]
[XmlType(AnonymousType = true)]
public class imagesTooltipsPlay
{
	private string textField;

	/// <remarks />
	public string text
	{
		get => textField;
		set => textField = value;
	}
}

/// <remarks />
[GeneratedCode(tool: "xsd", version: "2.0.50727.3038")]
[Serializable]
[DebuggerStepThrough]
[DesignerCategory(category: "code")]
[XmlType(AnonymousType = true)]
public class imagesTooltipsPause
{
	private string textField;

	/// <remarks />
	public string text
	{
		get => textField;
		set => textField = value;
	}
}