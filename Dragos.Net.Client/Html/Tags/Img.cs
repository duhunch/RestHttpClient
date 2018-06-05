
namespace Dragos.Net.Client.Html.Tags
{
    public class Img : SingleTag
    {
        public string Src
        {
            get
            {
                return Attributes["src"];
            }
        }
        public Img(string tagName, IAttributes attributes, DocInfo docInfo) : base(tagName, attributes, docInfo)
        {
        }
    }
}
