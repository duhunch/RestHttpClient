using Dragos.Net.Client.Html.Tags;

namespace Dragos.Net.Client.Html.Parsers
{
    public class DocumentTypeParser:IHtmlParser
    {
        public INode Parse(HtmlParser parser, HtmlPortion current)
        {
            if (current.IsStartTagChar() && current.IsNext('!'))
            {
                var text = current.Substring(' ');
                var t = text.ToLower() == "<!doctype ";
                if (t)
                {
                    var index = current.IndexOf('>');
                    current.Jump(index);
                    current.Next();
                    return new DocumentTypeTag();
                }
            }
          
            return null;
        }
    }
}
