using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Xml.Linq;
using Dragos.Net.Client.DataProviders;

namespace Dragos.Net.Client.DataProviders.XmlRpc
{
    public class RpcObjectXmlDataConverter :IXmlDataConverter
    {
        public bool Is(Type type)
        {
            return !type.IsArray && !new RpcStrucXmlDataConverter().Is(type) && type.IsClass;
        }


        public string GetValue(XmlDataProvider xmlDataProvider, object value)
        {
            var root = xmlDataProvider.StartTag("struct");
            foreach (var prop in value.GetType().GetProperties())
            {
                var propValue = (prop.GetValue(value));
                if (propValue == null) continue;
                root += xmlDataProvider.StartTag("member");
                root += xmlDataProvider.CreateNode("name", xmlDataProvider.GetName(prop));
                root += xmlDataProvider.CreateNode("value",xmlDataProvider.GetValue(propValue));
                root += xmlDataProvider.EndTag("member");
            }
            root += xmlDataProvider.EndTag("struct");
            return root;
        }

       

        public object Parse(XmlDataProvider xmlDataProvider, XElement element)
        {
            if (element.Name.LocalName != "value" || (element.FirstNode as XElement)?.Name.LocalName != "struct")
                return null;
                if (element.HasElements)
                {
                    var model = new XmlRpcObject();
                    var elements = element.Element("struct").Elements("member");
                    foreach (var elem in elements)
                    {
                        var name = elem.Element("name").Value;
                        model[name] = xmlDataProvider.Parse(elem.Element("value"));

                    }
                    return model;
                }
            return null;
        }

    }


}
