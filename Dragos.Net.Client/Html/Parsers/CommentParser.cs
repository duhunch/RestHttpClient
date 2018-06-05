using System.Text.RegularExpressions;
using Dragos.Net.Client.Html.Tags;

namespace Dragos.Net.Client.Html.Parsers
{
    public class CommentParser :IHtmlParser
    {
        private DocInfo _info;
        public CommentParser(DocInfo info)
        {
            this._info = info;
        }
       
        public INode Parse(HtmlParser parser, HtmlPortion current)
        {
            if (!IsValid(current)) return null;
            current.Next(4);
            var content = string.Empty;
            while (current.HasNext)
            {
                if (current.Is("-->"))
                {
                    current.Next(2);
                    break;
                }
                content += current.Char;
                current.Next();
            }
            current.Jump();
            return new Comment(content, _info);
        }

        private static bool IsValid(HtmlPortion current)
        {
            return (current.Is("<!--"));
        }
    }
}
