namespace zh.fang.stu.configuration
{
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Xml;

    internal interface ISectionHandle
    {

    }

    
    public sealed class SectionHandle:ConfigurationSection, ISectionHandle
    {
        [ConfigurationProperty("valueElement", IsRequired = false)]
        private ValueElement ValueElement => (ValueElement)base["valueElement"];

        [ConfigurationProperty("textElement", IsRequired = false)]
        private TextElement TextElement => (TextElement)base["textElement"];

        [ConfigurationProperty("cdataElement", IsRequired = false)]
        private CDataElement CDataElement => (CDataElement)base["cdataElement"];

        [ConfigurationProperty("errors", IsRequired = false)]
        private ErrorCollection Errors => (ErrorCollection)base["errors"];
    }

    internal class ValueElement:ConfigurationElement
    {
        protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
        {
            base.DeserializeElement(reader, serializeCollectionKey);
        }

        [ConfigurationProperty("value", IsRequired = true)]
        public string Value => (string)base["value"];
    }

    internal class TextElement:ConfigurationElement
    {
        protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
        {
            Text = reader.ReadElementContentAsString();
        }

        public string Text { get; private set; }
    }

    internal class CDataElement:ConfigurationElement
    {
        protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
        {
            Data = reader.ReadElementContentAsString();
        }

        public string Data { get; private set; }
    }

    internal class ErrorElement:ConfigurationElement
    {
        protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
        {
            base.DeserializeElement(reader, serializeCollectionKey);
        }

        [ConfigurationProperty("code", IsRequired = true)]
        public int Code => (int)base["code"];

        [ConfigurationProperty("msg", IsRequired = false, DefaultValue = null)]
        public string Msg => (string)base["msg"];
    }

    [ConfigurationCollection(typeof(ErrorElement), AddItemName = "add", CollectionType = ConfigurationElementCollectionType.BasicMap)]
    internal class ErrorCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ErrorElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ErrorElement)element).Code;
        }

        public ErrorElement[] Errors
        {
            get { return BaseGetAllKeys().Select(t => (ErrorElement)BaseGet(t)).ToArray(); }
        }
    }
}
