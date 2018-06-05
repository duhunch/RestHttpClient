

using System.Text.RegularExpressions;
using Dragos.Net.Client.Html.Exception;
using Dragos.Net.Client.Html.Tags;

namespace Dragos.Net.Client.Html.Parsers
{
    public class TagParser : IHtmlParser
    {
        private DocInfo _info;
        public TagParser(DocInfo info)
        {
            this._info = info;
        }

        public INode Parse(HtmlParser parser, HtmlPortion current)
        {
            if (current.IsWhiteSpace) return null;
            if (!IsValid(current)) return null;
            var attr = parser.GetAttribute(current);
            current.Jump();
            if (attr.IsSingle) return CreateSingle(attr.TagName, attr.Attributes);
            var tag = CreatePair(attr.TagName, attr.Attributes);
            foreach (var i in parser.Parse(current))
                tag.Append(i);
            var last = current.IndexOf(new Regex(@"</\s*" + attr.TagName + ">"));
            if (last == -1)
                throw new HtmlParseException(attr.TagName + " could not close");
            
            current.Jump(last);
            current.Jump();
            return tag;

        }

        private static bool IsValid(HtmlPortion current)
        {
            var result = current.Char == '<' && new Regex(@"[a-zA-Z]").IsMatch(current.NextChar.ToString());
            return result;
        }

        private PairTag CreatePair(string tagName, IAttributes attributes)
        {
            switch (tagName.ToLower())
            {
                case "div":
                    return new Div(tagName, attributes,_info);
                case "a":
                    return new A(tagName, attributes,_info);
                case "span":
                    return new Span(tagName, attributes,_info);
                case "li":
                    return new Li(tagName, attributes,_info);
                case "td":
                    return new Td(tagName, attributes,_info);
                case "tr":
                    return new Tr(tagName, attributes,_info);
                case "form":
                    return new Form(tagName, attributes,_info);
                case "select":
                    return new Select(tagName, attributes,_info);
                case "option":
                    return new Option(tagName, attributes,_info);
                case "table":
                    return new Table(tagName, attributes,_info);
                case "tbody":
                    return new TBody(tagName, attributes,_info);
                case "thead":
                    return new THead(tagName, attributes,_info);
                case "th":
                    return new Th(tagName, attributes,_info);
                case "ul":
                    return new Ul(tagName, attributes,_info);
                case "ol":
                    return new Ol(tagName, attributes,_info);
                case "title":
                    return new Title(tagName, attributes,_info);
                case "head":
                    return new Head(tagName, attributes,_info);
                case "body":
                    return new Body(tagName, attributes,_info);
                case "html":
                    return new Tags.Html(tagName, attributes, _info);
                default:
                        return new PairTag(tagName, attributes,_info);

            }

        }

        private SingleTag CreateSingle(string tagName,IAttributes attributes)
        {
            switch (tagName.ToLower())
            {
                case "input":
                    return new Input(tagName, attributes,_info);
                case "img":
                    return new Img(tagName, attributes,_info);
                case "link":
                    return new Link(tagName, attributes,_info);
                case "meta":
                    return new Meta(tagName, attributes,_info);
                default:
                    return new SingleTag(tagName, attributes, _info);

            }
        }
    }
}
