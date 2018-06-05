using Dragos.Net.Client.DataProviders;
using Dragos.Net.Client.DataProviders.XmlRpc;
using Dragos.Net.Client.Html;

namespace Dragos.Net.Client
{
    public interface IDataProvider
    {
        string GetValue(object value);
        object TryParse(string value);
    }

    public class DataProvider
    {
        public static IDataProvider XmlRpc => new XmlRpcDataProvider();


        public static IDataProvider Html(DocInfo docInfo)
        {
            return new HtmlObjectProvider(docInfo);
        }
    }

}
