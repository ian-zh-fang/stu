namespace zh.fang.stu.configuration
{
    using System;
    using System.Configuration;

    public class MyConfigSectionGroup:ConfigurationSectionGroup
    {
        public MyConfigSection Test => (MyConfigSection)Sections["testItems"];
    }

    public class MyConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("name", DefaultValue = "", IsRequired = true)]
        public string Name => (string)base["name"];

        [ConfigurationProperty("type", DefaultValue = "", IsRequired = true)]
        public string Type => (string)base["type"];

        [ConfigurationProperty("items", IsDefaultCollection = true, IsRequired = false)]
        public MyItemElementCollection Items => (MyItemElementCollection)base["items"];
    }

    public class MyItemElement : ConfigurationElement
    {
        [ConfigurationProperty("name", DefaultValue = "", IsRequired = true)]
        public string Name => (string)base["name"];

        [ConfigurationProperty("type", DefaultValue = "", IsRequired = true)]
        public string Type => (string)base["type"];
    }

    public class MyItemElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new MyItemElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MyItemElement)element).Name;
        }

        protected override string ElementName => "item";

        public override ConfigurationElementCollectionType CollectionType => ConfigurationElementCollectionType.BasicMap;

        public MyItemElement this[int index] => (MyItemElement)BaseGet(index);
    }
}
