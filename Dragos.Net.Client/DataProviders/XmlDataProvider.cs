using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Dragos.Data.Attributes;

namespace Dragos.Net.Client.DataProviders
{

    public interface IXmlNameValueProvider
    {
        string GetValue(string propertyName);
    }

    public class NameValueProvider:IXmlNameValueProvider
    {
        public string GetValue(string propertyName)
        {
            return propertyName;
        }
    }
    public interface IXmlDataConverter
    {
        bool Is(Type type);
        string GetValue(XmlDataProvider xmlDataProvider, object value);
        object Parse(XmlDataProvider xmlDataProvider,XElement element);
    }

    public abstract class XmlDataProvider :IDataProvider
    {
        private readonly List<IXmlDataConverter> _converters = new List<IXmlDataConverter>();

        private IXmlNameValueProvider _nameValueProvider = new NameValueProvider();

        public IXmlDataConverter[] Converters => _converters.ToArray();


        public IXmlNameValueProvider NameValueProvider
        {
            get { return _nameValueProvider; }
            set
            {
                if (value == null) return;
                _nameValueProvider = value;
            }
        }

        public bool NullValueShown { get; set; } = false;

        public XmlDataProvider AddConverter(IXmlDataConverter converter)
        {
            this._converters.Add(converter);
            return this;
        }

        public void Remove(IXmlDataConverter converter)
        {
            var item = this._converters.FirstOrDefault(x => x == converter);
            this._converters.Remove(item);
        }


        public void RemoveAll()
        {
           this._converters.Clear();
        }

        public string GetValue(object value)
        {
            return GetValue(this, value);
        }

       public string GetName(Type type)
        {
            if (CheckIfAnonymousType(type))
                return "item";
            var nameAttr =type.GetCustomAttribute<DataNameAttribute>();
            if (nameAttr == null)
                return type.Name;
            return nameAttr.Name;
        }

        public string GetName(PropertyInfo info)
        {
            var nameAttr = info.GetCustomAttribute<DataNameAttribute>();
            if (nameAttr == null)
                return info.Name;
            return nameAttr.Name;
        }



        private static bool CheckIfAnonymousType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            // HACK: The only way to detect anonymous types right now.
            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                && type.IsGenericType && type.Name.Contains("AnonymousType")
                && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }

        public string CreateNode(string name, string value)
        {
            return StartTag(name) + value + EndTag(name);
        }

        public string StartTag(string name)
        {
            return "<" +NameValueProvider.GetValue(name) + ">";
        }

        public string EndTag(string name)
        {
            return "</" + NameValueProvider.GetValue(name) + ">";
        }

        public bool Is(Type type)
        {
            return this._converters.Any(x => x.Is(type));
        }

        public string GetValue(XmlDataProvider xmlDataProvider, object value)
        {
            if (value == null) return "";
            var result = "";
            foreach (var converter in this._converters)
            {
                if (converter.Is(value.GetType()))
                {
                    result = converter.GetValue(this, value);
                    return result;
                }
            }
            return result;
        }

        public object TryParse(string value)
        {
            var xElement = XElement.Parse(value);
            if (xElement.Name.LocalName == "xml")
                return Parse(xElement.FirstNode as XElement);
            return Parse(xElement);
        }

   

        public object Parse(XElement element)
        {
            foreach (var convert in _converters)
            {
                var item = convert.Parse(this, element);
                if (item != null) return item;
            }
            return null;

        }

    }
}
