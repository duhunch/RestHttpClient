using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Dragos.Data.Attributes;
using Dragos.Net.Client.DataProviders;

namespace Dragos.Net.Client.DataProviders.XmlRpc
{
    internal class RpcStrucXmlDataConverter:IXmlDataConverter
    {
        public bool Is(Type type)
        {
            var other = new[] { typeof(string) };
            return (type.IsValueType || other.Any(x => type == x));
        }

        public string GetValue(XmlDataProvider xmlDataProvider, object value)
        {
            if (value == null) return "";
            return xmlDataProvider.CreateNode(GetTypeName(value), value.ToString());
        }

        private string GetTypeName(object value)
        {
            var type = value.GetType();
            var attribute = type.GetCustomAttribute<DataTypeAttribute>();
            if (attribute == null)
                return GetTypeName(Type.GetTypeCode(value.GetType()));
            return GetTypeName(attribute.TypeCode);
        }
        private string GetTypeName(TypeCode code)
        {
            if (code == TypeCode.Int32 || code == TypeCode.Int16 || code == TypeCode.Int64)
                return "int";
            if ((code == TypeCode.String))
                return "string";
            if (code == TypeCode.DateTime)
                return "dateTime.iso8601";
            if (code == TypeCode.Double || code == TypeCode.Decimal)
                return "double";
            return "string";
        }

        public object Parse(XmlDataProvider xmlDataProvider, XElement element)
        {
            if (element.Name.LocalName != "value") return null;
            return Parse(element.FirstNode as XElement);
        }

        private static object Parse(XElement element)
        {
            var name = element.Name.LocalName;
            var value = element.Value;
            switch (name)
            {
                case "int":
                    return Convert.ToInt32(value);
                case "string":
                    return Convert.ToString(value);
                case "double":
                    return Convert.ToDouble(value);
                default:
                    if (name.StartsWith("dateTime"))
                        return DateTime.Parse(value);
                    return null;
            }
        }
    }
}
