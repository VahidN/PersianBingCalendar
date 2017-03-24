using System.Xml.Serialization;

namespace PersianBingCalendar.Models
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public partial class images
    {

        private imagesImage imageField;

        private imagesTooltips tooltipsField;

        /// <remarks/>
        public imagesImage image
        {
            get
            {
                return this.imageField;
            }
            set
            {
                this.imageField = value;
            }
        }

        /// <remarks/>
        public imagesTooltips tooltips
        {
            get
            {
                return this.tooltipsField;
            }
            set
            {
                this.tooltipsField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType = true)]
    public partial class imagesImage
    {

        private uint startdateField;

        private ulong fullstartdateField;

        private uint enddateField;

        private string urlField;

        private string urlBaseField;

        private string copyrightField;

        private string copyrightlinkField;

        private byte drkField;

        private byte topField;

        private byte botField;

        private object hotspotsField;

        /// <remarks/>
        public uint startdate
        {
            get
            {
                return this.startdateField;
            }
            set
            {
                this.startdateField = value;
            }
        }

        /// <remarks/>
        public ulong fullstartdate
        {
            get
            {
                return this.fullstartdateField;
            }
            set
            {
                this.fullstartdateField = value;
            }
        }

        /// <remarks/>
        public uint enddate
        {
            get
            {
                return this.enddateField;
            }
            set
            {
                this.enddateField = value;
            }
        }

        /// <remarks/>
        public string url
        {
            get
            {
                return this.urlField;
            }
            set
            {
                this.urlField = value;
            }
        }

        /// <remarks/>
        public string urlBase
        {
            get
            {
                return this.urlBaseField;
            }
            set
            {
                this.urlBaseField = value;
            }
        }

        /// <remarks/>
        public string copyright
        {
            get
            {
                return this.copyrightField;
            }
            set
            {
                this.copyrightField = value;
            }
        }

        /// <remarks/>
        public string copyrightlink
        {
            get
            {
                return this.copyrightlinkField;
            }
            set
            {
                this.copyrightlinkField = value;
            }
        }

        /// <remarks/>
        public byte drk
        {
            get
            {
                return this.drkField;
            }
            set
            {
                this.drkField = value;
            }
        }

        /// <remarks/>
        public byte top
        {
            get
            {
                return this.topField;
            }
            set
            {
                this.topField = value;
            }
        }

        /// <remarks/>
        public byte bot
        {
            get
            {
                return this.botField;
            }
            set
            {
                this.botField = value;
            }
        }

        /// <remarks/>
        public object hotspots
        {
            get
            {
                return this.hotspotsField;
            }
            set
            {
                this.hotspotsField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType = true)]
    public partial class imagesTooltips
    {

        private imagesTooltipsLoadMessage loadMessageField;

        private imagesTooltipsPreviousImage previousImageField;

        private imagesTooltipsNextImage nextImageField;

        private imagesTooltipsPlay playField;

        private imagesTooltipsPause pauseField;

        /// <remarks/>
        public imagesTooltipsLoadMessage loadMessage
        {
            get
            {
                return this.loadMessageField;
            }
            set
            {
                this.loadMessageField = value;
            }
        }

        /// <remarks/>
        public imagesTooltipsPreviousImage previousImage
        {
            get
            {
                return this.previousImageField;
            }
            set
            {
                this.previousImageField = value;
            }
        }

        /// <remarks/>
        public imagesTooltipsNextImage nextImage
        {
            get
            {
                return this.nextImageField;
            }
            set
            {
                this.nextImageField = value;
            }
        }

        /// <remarks/>
        public imagesTooltipsPlay play
        {
            get
            {
                return this.playField;
            }
            set
            {
                this.playField = value;
            }
        }

        /// <remarks/>
        public imagesTooltipsPause pause
        {
            get
            {
                return this.pauseField;
            }
            set
            {
                this.pauseField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType = true)]
    public partial class imagesTooltipsLoadMessage
    {

        private string messageField;

        /// <remarks/>
        public string message
        {
            get
            {
                return this.messageField;
            }
            set
            {
                this.messageField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType = true)]
    public partial class imagesTooltipsPreviousImage
    {

        private string textField;

        /// <remarks/>
        public string text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType = true)]
    public partial class imagesTooltipsNextImage
    {

        private string textField;

        /// <remarks/>
        public string text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType = true)]
    public partial class imagesTooltipsPlay
    {

        private string textField;

        /// <remarks/>
        public string text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType = true)]
    public partial class imagesTooltipsPause
    {

        private string textField;

        /// <remarks/>
        public string text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }
}