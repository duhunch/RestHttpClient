using System.Text.RegularExpressions;
using Dragos.Net.Client.Html.Exception;
using Dragos.Net.Client.Html.Tags;

namespace Dragos.Net.Client.Html.Parsers
{
    public class TextParser :IHtmlParser
    {
        private DocInfo _info;
        public TextParser(DocInfo info)
        {
            this._info = info;
        }

        public INode Parse(HtmlParser parser, HtmlPortion current)
        {
            if (char.IsWhiteSpace(current.Char)) return null;
            if (IsHtml(current)) return null;
            var result = string.Empty;
            while (current.HasNext)
            {
                if (IsHtml(current))
                    return Return(result);
                
                result += current.Char;
                current.Next();
            }
            throw new HtmlParseException("text is not valid for parse");
        }

        private bool IsHtml(HtmlPortion current)
        {
            if (current.Is("<!--")) return true;
            return new Regex(@"^</?[a-zA-Z]").IsMatch(current.GetText(3));
        }

        private Text Return(string result)
        {
            if (string.IsNullOrWhiteSpace(result)) return null;
            return new Text(result, _info);
        }
    }
}
