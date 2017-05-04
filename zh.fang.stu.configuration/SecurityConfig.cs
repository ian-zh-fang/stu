namespace zh.fang.stu.configuration
{
    using System.Configuration;

    public class SecurityProviderSection:ConfigurationSection
    {
        [ConfigurationProperty("defaultProviderName", IsRequired = true)]
        public string DefaultProviderName => (string)this["defaultProviderName"];

        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        public ProviderCollection Collection => (ProviderCollection)this[""];
    }


    [ConfigurationCollection(typeof(ProviderElement), AddItemName = "provider")]
    public class ProviderCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ProviderElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ProviderElement)element).Name;
        }
    }

    public class ProviderElement:ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name => (string)this["name"];

        [ConfigurationProperty("type", IsRequired = true)]
        public string Type => (string)this["type"];

        [ConfigurationProperty("filters", IsRequired = false)]
        public FilterCollection Collection => (FilterCollection)this["filters"];
    }

    [ConfigurationCollection(typeof(FilterElement), AddItemName = "add", ClearItemsName = "clear")]
    public class FilterCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new FilterElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((FilterElement)element).Type;
        }
    }

    public class FilterElement:ConfigurationElement
    {
        [ConfigurationProperty("type", IsRequired = true)]
        public string Type => (string)this["type"];
    }
}
