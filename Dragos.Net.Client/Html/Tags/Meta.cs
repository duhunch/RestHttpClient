namespace Dragos.Net.Client.Html.Tags
{
    public class Meta : SingleTag,IHead
    {
        public Meta(string tagName, IAttributes attributes,DocInfo docInfo) : base(tagName, attributes,docInfo)
        {
        }
    }
}
