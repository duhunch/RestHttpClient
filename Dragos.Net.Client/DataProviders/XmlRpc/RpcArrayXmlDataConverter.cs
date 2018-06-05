using System;
using System.Collections;
using System.Linq;
using System.Xml.Linq;
using Dragos.Net.Client.DataProviders;

namespace Dragos.Net.Client.DataProviders.XmlRpc
{
     public class RpcArrayXmlDataConverter :IXmlDataConverter
    {
        public bool Is(Type type)
        {
            return type.IsArray;
        }

        public string GetValue(XmlDataProvider xmlDataProvider, object value)
        {
          
            var root = xmlDataProvider.StartTag("array");
            root += xmlDataProvider.StartTag("data");
            root = ((IEnumerable)value).Cast<object>().Aggregate(root, (current, item) => current + xmlDataProvider.CreateNode("value", xmlDataProvider.GetValue(item).ToString()));
            root += xmlDataProvider.EndTag("data");
            root += xmlDataProvider.EndTag("array");
            return root;
        }

        public object Parse(XmlDataProvider xmlDataProvider, XElement element)
        {
            if (element.Name.LocalName != "value" || (element.FirstNode as XElement)?.Name.LocalName != "array") return null;
            var array = new ArrayList();
            foreach (var elem in (element.FirstNode as XElement).Element("data").Elements())
            {
                array.Add(xmlDataProvider.Parse(elem));
            }
            return array;
        }
    }
}
