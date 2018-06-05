
using System;

namespace Dragos.Net.Client.DataProviders.XmlRpc
{
    public class XmlRpc : BaseRequest
    {

        public XmlRpc(WebClient client,string url):base(client,new Uri(url))
        {
            ContentProviders.Add("application/xml", new XmlRpcDataProvider());
            ContentProviders.Add("text/xml", new XmlRpcDataProvider());
            ContentProviders.Add("text/html", new HtmlObjectProvider(new Html.DocInfo(client,(this.Url))));

        }
        public Response Invoke(string methodName, params object[] parameters)
        {
            return GetResponse(RequestMethod.Post, new MethodCall(methodName, parameters));
        }
    }
}
