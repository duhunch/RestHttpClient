using Dragos.Net.Client.DataProviders;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Xml.Linq;

namespace Dragos.Net.Client.DataProviders.XmlRpc
{
    public class RpcMethodCallXmlDataConverter :IXmlDataConverter
    {
        public bool Is(Type type)
        {
            return type == typeof(MethodCall);
        }

        public string GetValue(XmlDataProvider xmlDataProvider,  object value)
        {
            var method = (MethodCall) value;
            var root = xmlDataProvider.StartTag("methodCall");
            root += xmlDataProvider.CreateNode("methodName", method.MethodName);
            root += xmlDataProvider.StartTag("params");

            foreach (var param in method.Params)
                root += xmlDataProvider.CreateNode("param",
                    xmlDataProvider.CreateNode("value", xmlDataProvider.GetValue(param)));
            
            root += xmlDataProvider.EndTag("params");
            root += xmlDataProvider.EndTag("methodCall");
            return root;
        }

        public object Parse(XmlDataProvider xmlDataProvider, XElement element)
        {
            var isResponse = TryParseIfMethodResponse(xmlDataProvider, element);
            if (isResponse != null) return isResponse;
            return TryParseIfMethodCall(xmlDataProvider, element);
        }

        private static object TryParseIfMethodResponse(XmlDataProvider xmlDataProvider, XElement element)
        {
            if (element.Name.LocalName != "methodResponse") return null;
            dynamic methodResponse = new XmlRpcObject();
            var resultOk = (element.Element("params")) != null;
            if (resultOk)
            {
                methodResponse.Result = true;
                methodResponse.Params = new List<dynamic>();
                foreach (var param in element.Element("params").Elements("param"))
                {
                    var i = (xmlDataProvider.Parse(param.Element("value")));
                    methodResponse.Params.Add(i);
                }
                return methodResponse;
            }
            var isFault = (element.Element("fault")) != null;
            if (isFault)
            {
                methodResponse.Result = false;
                methodResponse.fault = xmlDataProvider.Parse(element.Element("fault").FirstNode as XElement);
                return methodResponse;
            }
            return null;

        }

        private static object TryParseIfMethodCall(XmlDataProvider xmlDataProvider, XElement element)
        {
            if (element.Name.LocalName != "methodCall") return null;
            dynamic methodCall = new ExpandoObject();
            methodCall.methodName = element.Element("methodName").Value;
            var parameters = element.Element("params");
            var list = new List<dynamic>();
            methodCall["params"] = list;
            foreach (var parameter in parameters.Elements("param"))
            {
                var value = parameter.Element("value");
                list.Add(xmlDataProvider.Parse(value));
            }
            return methodCall;
        }
    }
}
