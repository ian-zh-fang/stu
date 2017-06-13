namespace zh.fang.stu.configuration
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    internal interface IConfig
    {
        string ApiUri { get; }

        string GatewayUri { get; }

        string PlatformNo { get; }

        int KeySerial { get; }

        string PublickKey { get; }

        string PrivateKey { get; }

        IErrorCollection Errors { get; }
    }

    internal interface IError
    {
        int Code { get; }

        string Message { get; }
    }

    internal interface IErrorCollection
    {
        IError Get(int code);

        IEnumerable<IError> Errors { get; }
    }

    internal sealed class ErrorElementc : ConfigurationElement, IError
    {
        [ConfigurationProperty("code", IsRequired = true)]
        int IError.Code
        {
            get { return (int)this["code"]; }
        }

        [ConfigurationProperty("msg", DefaultValue = null, IsRequired = false)]
        string IError.Message
        {
            get { return (string)this["msg"]; }
        }
    }

    [ConfigurationCollection(typeof(ErrorElementc), AddItemName = "add", ClearItemsName = "clear")]
    internal sealed class ErrorElementCollection : ConfigurationElementCollection, IErrorCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ErrorElementc();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((IError)element).Code;
        }

        IError IErrorCollection.Get(int code)
        {
            return (IError)BaseGetAllKeys().FirstOrDefault(t => code == (int)t);
        }

        IEnumerable<IError> IErrorCollection.Errors
        {
            get
            {
                return BaseGetAllKeys().Select(t => (IError)BaseGet(t));
            }
        }
    }


    internal class StringValueElement : ConfigurationElement
    {
        [ConfigurationProperty("value", IsRequired = true)]
        public String Value
        {
            get { return (String)base["value"]; }
        }
    }

    internal class Int32ValueElement : ConfigurationElement
    {
        [ConfigurationProperty("value", IsRequired = true)]
        public Int32 Value
        {
            get { return (Int32)base["value"]; }
        }
    }

    public sealed class ConfigurationSectionHandle : ConfigurationSection, IConfig
    {
        // impl

        [ConfigurationProperty("apiUri", IsRequired = false)]
        private StringValueElement ApiUri
        {
            get { return (StringValueElement)base["apiUri"]; }
        }

        [ConfigurationProperty("gatewayUri", IsRequired = false)]
        private StringValueElement GatewayUri
        {
            get { return (StringValueElement)base["gatewayUri"]; }
        }

        [ConfigurationProperty("platformNo", IsRequired = false)]
        private StringValueElement PlatformNo
        {
            get { return (StringValueElement)base["platformNo"]; }
        }

        [ConfigurationProperty("keySerial", IsRequired = false)]
        private Int32ValueElement KeySerial
        {
            get { return (Int32ValueElement)base["keySerial"]; }
        }

        [ConfigurationProperty("publickKey", IsRequired = false)]
        private TextElement PublickKey
        {
            get { return (TextElement)base["publickKey"]; }
        }

        [ConfigurationProperty("privateKey", IsRequired = false)]
        private TextElement PrivateKey
        {
            get { return (TextElement)base["privateKey"]; }
        }

        [ConfigurationProperty("errors", IsRequired = false)]
        private ErrorElementCollection Errors
        {
            get { return (ErrorElementCollection)base["errors"]; }
        }

        // interface 

        string IConfig.ApiUri
        {
            get { return ApiUri.Value; }
        }

        string IConfig.GatewayUri
        {
            get { return GatewayUri.Value; }
        }

        string IConfig.PlatformNo
        {
            get { return PlatformNo.Value; }
        }

        int IConfig.KeySerial
        {
            get { return KeySerial.Value; }
        }

        string IConfig.PublickKey
        {
            get { return PublickKey.Text; }
        }

        string IConfig.PrivateKey
        {
            get { return PrivateKey.Text; }
        }

        IErrorCollection IConfig.Errors
        {
            get { return Errors; }
        }
    }
}
