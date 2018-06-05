namespace Dragos.Net.Client.Html.Tags
{
    public class Link : SingleTag,IHead
    {
        public Link(string tagName, IAttributes attributes,DocInfo docInfo) : base(tagName, attributes,docInfo)
        {
        }

        public Response Get()
        {
            return DocInfo.Client.GetRequest(this.Attributes["href"]).Get();
        }
    }
}
