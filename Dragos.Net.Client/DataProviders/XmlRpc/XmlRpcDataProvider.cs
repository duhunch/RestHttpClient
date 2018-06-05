
using Dragos.Net.Client.DataProviders;

namespace Dragos.Net.Client.DataProviders.XmlRpc
{
    public class XmlRpcDataProvider :XmlDataProvider
    {
        public XmlRpcDataProvider()
        {
            this.AddConverter(new RpcMethodCallXmlDataConverter());
            this.AddConverter(new RpcStrucXmlDataConverter());
            this.AddConverter(new RpcObjectXmlDataConverter());
            this.AddConverter(new RpcArrayXmlDataConverter());
        }
    }
}
