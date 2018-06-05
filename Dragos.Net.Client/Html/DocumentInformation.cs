
namespace Dragos.Net.Client.Html
{
    public class DocInfo
    {
        public System.Uri Uri { get; }

        public WebClient Client { get; }

        public DocInfo(WebClient client,System.Uri uri)
        {
            this.Uri = uri;
            Client = client;
        }
    }
}
