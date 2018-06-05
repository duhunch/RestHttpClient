using Dragos.Net.Client.Html.Tags;

namespace Dragos.Net.Client.Html
{

    public class HtmlDocument:PairTag
    {
        public PairTag Document { get; private set; }

        public HtmlDocument(DocInfo docInfo) : base("html", new Attributes(),docInfo)
        {
        }

        public override void Append(INode node)
        {
            if (Document == null)
            {
                var nullable = (node as PairTag);
                if (nullable != null)
                    if (nullable.TagName.ToLower() == "html")
                        Document = nullable;
            }
            base.Append(node);
        }
    }
}
