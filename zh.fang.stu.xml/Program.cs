using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Collections;

namespace zh.fang.stu.xml
{
    class Program
    {
        static void Main(string[] args)
        {
            var cfg = new MyConfiguration("myconfig.config");
            var rdm = new Random();
            foreach (var item in cfg.Pie)
            {
                item.Id = RandomValue(rdm);
                item.Name = RandomValue(rdm).ToString();
            }

            foreach (var item in cfg.Mom)
            {
                foreach (var sitem in item)
                {
                    sitem.Id = RandomValue(rdm);
                    sitem.Name = RandomValue(rdm).ToString();
                }
            }
            cfg.SaveAs();
        }

        private static int RandomValue(Random rdm = null)
        {
            rdm = rdm ?? new Random();
            var buffer = new byte[4];
            rdm.NextBytes(buffer);

            var value = 0;
            for (int i = 0; i < buffer.Length; i++)
            {
                value |= buffer[i];
                value <<= (i + 1) * 8;
            }

            return value;
        }

        private class MyConfiguration : XmlConfiguration
        {
            public MyConfiguration(string file, string section) : base(file, section) { }

            public MyConfiguration(string fileName)
                :this(string.Format(@"{0}\{1}", Environment.CurrentDirectory, fileName), "configuration")
            { }

            public MyConfiguration()
                : this(string.Format(@"{0}.exe.config", typeof(XmlConfigurationElement).Assembly.GetName().Name))
            { }

            protected override bool OnDeserializeUnrecognizeAttribute(string name, XmlReader reader)
            {
                Console.WriteLine(reader.Value);
                return true;
            }

            protected override bool OnDeserializeUnrecognizeElement(string name, XmlReader reader)
            {
                Console.WriteLine(reader.ReadOuterXml());
                return true;
            }

            public XmlConfigurationProperty<Int32> a { get; private set; }

            public XmlConfigurationStartup Startup { get; private set; }

            public XmlConfigurationPie Pie { get; private set; }

            public XmlConfigurationMom Mom { get; private set; }
        }

        private class XmlConfigurationItem : XmlConfigurationElement
        {
            public string Name { get; set; }

            public int Id { get; set; }
        }

        private class XmlConfigurationMomItem : XmlConfigurationArray<XmlConfigurationItem>
        {
            public string Name { get; set; }
        }

        private class XmlConfigurationMom : XmlConfigurationArray<XmlConfigurationMomItem>
        {
            public XmlConfigurationMom() : base("item") { }
        }

        private class XmlConfigurationPie : XmlConfigurationArray<XmlConfigurationItem> { }

        private class XmlConfigurationArray<TItem>:XmlConfigurationElement, IEnumerable<TItem>
            where TItem:XmlConfigurationElement
        {
            private readonly string _addElementName;
            private readonly string _removeElementName;
            private readonly string _clearElementName;

            private readonly List<TItem> _collection = new List<TItem>();

            public XmlConfigurationArray(string addElementName, string removeElementName = "remove", string clearElementName = "clear") : base()
            {
                _addElementName = addElementName;
                _removeElementName = removeElementName;
                _clearElementName = clearElementName;
            }

            public XmlConfigurationArray() : this("add") { }

            public void Add(TItem item)
            {
                _collection.Add(item);
            }

            public void AddRange(IEnumerable<TItem> items)
            {
                _collection.AddRange(items);
            }

            protected override void OnSerialize(XmlWriter writer)
            {
                base.OnSerialize(writer);
                _collection.ForEach(t =>
                {
                    writer.WriteStartElement(_addElementName);
                    t.Serialize(writer);
                    writer.WriteEndElement();
                });
            }

            protected override void OnDeserialze(XmlReader reader, PropertyInfo[] properties)
            {
                var name = reader.Name;
                if (_addElementName == name)
                {
                    OnDeserializeAddElement(reader);
                    return;
                }

                if (_removeElementName == name)
                {
                    OnDeserializeRemoveElement(reader);
                    return;
                }

                if(_clearElementName == name)
                {
                    OnDeserializeClearElement(reader);
                    return;
                }

                if (OnDeserializeUnrecognizeElement(name, reader)) { return; }
                throw new Exception("Unrecognize element dose not processed .");
            }

            protected virtual void OnDeserializeAddElement(XmlReader reader)
            {
                var itemType = typeof(TItem);
                var type = GetElementType(reader, itemType);
                if (!itemType.IsAssignableFrom(type)) { throw new NotSupportedException(); }

                var element = Activator.CreateInstance(type) as TItem;
                element.Deserialize(reader);
                _collection.Add(element);
            }

            protected virtual void OnDeserializeRemoveElement(XmlReader reader) { }

            protected virtual void OnDeserializeClearElement(XmlReader reader)
            {
                _collection.Clear();
            }

            IEnumerator<TItem> IEnumerable<TItem>.GetEnumerator()
            {
                return _collection.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable<TItem>)this).GetEnumerator();
            }
        }

        private class XmlConfigurationStartup:XmlConfigurationElement
        {
            public XmlConfigurationSupportedRuntime SupportedRuntime { get; private set; }
        }

        private class XmlConfigurationSupportedRuntime : XmlConfigurationElement
        {
            public string Version { get; private set; }

            public string Sku { get; private set; }
        }

        private class XmlConfigurationProperty<TValue>:XmlConfigurationElement
        {
            public XmlConfigurationProperty() : base() { }

            public TValue Value { get; private set; }
        }

        private sealed class XmlConfigurationPropertyInt32: XmlConfigurationProperty<Int32>
        {
            public XmlConfigurationPropertyInt32() : base() { }

            protected override object GetValue(string valueStr, Type valueType)
            {
                return Int32.Parse(valueStr);
            }
        }

        private abstract class XmlConfiguration: XmlConfigurationElement
        {
            private readonly string _file;
            private readonly string _section;

            protected XmlConfiguration(string filepath, string section)
            {
                if (string.IsNullOrWhiteSpace(section)) { throw new ArgumentNullException(nameof(section)); }
                _file = filepath;
                _section = section;

                using (var reader = GetReader(filepath, section))
                {
                    Deserialize(reader);
                }
            }

            public void SaveAs()
            {
                using (var writer = XmlWriter.Create(_file))
                {
                    writer.WriteStartDocument();
                    SerializeSection(writer);
                    writer.WriteEndDocument();
                }
            }

            private void SerializeSection(XmlWriter writer)
            {
                writer.WriteStartElement(_section);
                Serialize(writer);
                writer.WriteFullEndElement();
            }

            protected virtual XmlReader GetReader(string filePath, string section)
            {
                var reader = XmlReader.Create(filePath);
                if (string.IsNullOrWhiteSpace(section)) { return reader; }

                section = section.ToLower();
                while(reader.Read())
                {
                    if (section == reader.Name.ToLower() && XmlNodeType.Element == reader.NodeType) { break; }
                }
                return reader;
            }
        }

        private abstract class XmlConfigurationElement
        {
            internal void Serialize(XmlWriter writer)
            {
                OnSerialize(writer);
            }

            protected virtual void OnSerialize(XmlWriter writer)
            {
                var arr = GetProperties();
                var elementType = typeof(XmlConfigurationElement);
                foreach (var pro in arr)
                {
                    var value = pro.GetValue(this, null);
                    if (null == value) { continue; }

                    var name = GetPropertyName(pro);
                    if (elementType.IsAssignableFrom(pro.PropertyType))
                    {
                        OnSerializeElement(name, value as XmlConfigurationElement, writer, pro);
                        continue;
                    }

                    OnSerializeAttribute(name, value, writer, pro);
                }
            }

            protected virtual void OnSerializeAttribute(string elementName, object value, XmlWriter writer, PropertyInfo property)
            {
                writer.WriteAttributeString(elementName, value.ToString());
            }

            protected virtual void OnSerializeElement(string elementName, XmlConfigurationElement value, XmlWriter writer, PropertyInfo property)
            {
                writer.WriteStartElement(elementName);
                value.Serialize(writer);
                writer.WriteFullEndElement();
            }

            internal void Deserialize(XmlReader reader)
            {
                var arr = GetProperties();
                DeserializeAttribute(reader, arr);
                Deserialize(reader, arr);
            }

            private void DeserializeAttribute(XmlReader reader, PropertyInfo[] properties)
            {
                var size = reader.AttributeCount;
                for (int i = 0; i < size; i++)
                {
                    reader.MoveToAttribute(i);
                    OnDeserializeAttribute(reader, properties);
                }
                if (reader.HasAttributes) { reader.MoveToElement(); }
            }

            private void OnDeserializeAttribute(XmlReader reader, PropertyInfo[] properties)
            {
                var name = reader.Name;
                var pro = GetProperty(name, properties);
                if(null == pro)
                {
                    if (OnDeserializeUnrecognizeAttribute(name, reader)) { return; }
                    throw new Exception("Unrecognize attribute dose not processed .");
                }

                var val = GetValue(reader.Value, pro.PropertyType);
                SetProperty(val, pro);
            }

            protected virtual object GetValue(string valueStr, Type valueType)
            {
                if (valueType == typeof(string)) { return valueStr; }
                if (valueType.IsPrimitive) { return ToPrimitiveValue(valueStr, valueType); }
                if (valueType.IsClass && 
                    valueType.IsGenericType && 
                    valueType.GetGenericTypeDefinition() == typeof(Nullable<>)) { return ToNullableValue(valueStr, valueType); }
                
                return Convert.ChangeType(valueStr, valueType);
            }

            private object ToPrimitiveValue(string valueStr, Type valueType)
            {
                const string _Name = "Parse";
                var method = valueType.GetMethod(_Name,
                    BindingFlags.Public | BindingFlags.Static, Type.DefaultBinder, new Type[] { typeof(string) }, new ParameterModifier[] { new ParameterModifier(1) });
                if (null == method) { return null; }

                return method.Invoke(null, new object[] { valueStr });
            }

            private object ToNullableValue(string valueStr, Type valueType)
            {
                var rawType = valueType.GenericTypeArguments.First();
                var rawValue = GetValue(valueStr, rawType);

                var actor = valueType.GetConstructor(BindingFlags.Public, Type.DefaultBinder, new Type[] { rawType }, new ParameterModifier[] { new ParameterModifier(1) });
                if (null == actor) { return null; }

                return actor.Invoke(new object[] { rawValue });
            }

            private void Deserialize(XmlReader reader, PropertyInfo[] properties)
            {
                // 空元素停止处理
                if (reader.IsEmptyElement) { return; }

                // 处理非空元素
                var currentRoot = reader.Name;
                while (reader.Read())
                {
                    var ndType = reader.NodeType;
                    if (currentRoot == reader.Name && ndType == XmlNodeType.EndElement) { break; }
                    if (ndType != XmlNodeType.Element) { continue; }
                    OnDeserialze(reader, properties);
                }
            }

            protected virtual void OnDeserialze(XmlReader reader, PropertyInfo[] properties)
            {
                var name = reader.Name;
                var pro = GetProperty(name, properties);
                if (null == pro)
                {
                    if (OnDeserializeUnrecognizeElement(name, reader)) { return; }
                    throw new Exception("Unrecognize element dose not processed .");
                }

                var element = CreateElement(reader, pro);
                element.Deserialize(reader);
                SetProperty(element, pro);
            }

            private void SetProperty(object value, PropertyInfo property)
            {
                if (null == value) { return; }
                var method = 
                    property.SetMethod ?? 
                    property.DeclaringType.GetProperty(property.Name, BindingFlags.Public | BindingFlags.Instance).SetMethod;
                if (null == method) { return; }

                method.Invoke(this, new object[] { value });
            }

            protected virtual XmlConfigurationElement CreateElement(XmlReader reader, PropertyInfo property)
            {
                var type = GetElementType(reader, property.PropertyType);
                if (type.IsSubclassOf(typeof(XmlConfigurationElement)))
                {
                    return Activator.CreateInstance(type) as XmlConfigurationElement;
                }

                throw new NotSupportedException();
            }

            protected Type GetElementType(XmlReader reader, Type defaultType)
            {
                var rawType = defaultType;
                const string __Attr = "type";
                if(reader.MoveToAttribute(__Attr))
                {
                    var typeStr = reader.Value;
                    rawType = Type.GetType(typeStr);
                    reader.MoveToElement();
                }

                return rawType;
            }

            private PropertyInfo GetProperty(string name, PropertyInfo[] properties)
            {
                name = name.ToLower();
                return
                    properties.FirstOrDefault(t => name == GetPropertyName(t));
            }

            protected virtual string GetPropertyName(PropertyInfo property)
            {
                return property.Name.ToLower();
            }

            private PropertyInfo[] GetProperties(BindingFlags flags = BindingFlags.Public| BindingFlags.Instance)
            {
                return
                    GetType().GetProperties(flags);
            }

            protected virtual bool OnDeserializeUnrecognizeAttribute(string name, XmlReader reader) { return false; }

            protected virtual bool OnDeserializeUnrecognizeElement(string name, XmlReader reader) { return false; }
        }
    }
}
