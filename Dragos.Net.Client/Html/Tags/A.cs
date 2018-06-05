namespace Dragos.Net.Client.Html.Tags
{
    public class A : PairTag
    {
        public string Href { get
            {
                return Attributes["href"];
            } }
        public A(string tagName, IAttributes attributes, DocInfo docInfo) : base(tagName, attributes, docInfo)
        {
        }

        public Response Get()
        {
            return DocInfo.Client.GetRequest(this.Attributes["href"]).Get();
        }
    }
}
