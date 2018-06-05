using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Dragos.Net.Client.Html.Parsers;

namespace Dragos.Net.Client.Html
{
    public class HtmlParser
    {
        public DocInfo Info { get; private set; }
        public HtmlParser(DocInfo info)
        {
            Info = info;
        }

        public HtmlDocument Parse(string htmlRaw)
        {
            htmlRaw = EscapeHtml(htmlRaw);
            var htmlDocument = new HtmlDocument(this.Info);
            var htmlPortion = new HtmlPortion(htmlRaw.TrimStart());
            foreach (var node in Parse(htmlPortion,new IHtmlParser[] {new DocumentTypeParser(), new TagParser(this.Info),new EndTagParser() }))
            {
                htmlDocument.Append(node);
            }
            return htmlDocument;
        }

        private static string EscapeHtml(string htmlRaw)
        {
            return System.Net.WebUtility.HtmlDecode(htmlRaw);
        }

        public static IEnumerable<INode> Partial(string htmlRaw,DocInfo info = null)
        {
            htmlRaw = EscapeHtml(htmlRaw);
            var htmlPortion = new HtmlPortion(htmlRaw);
            htmlPortion.Jump();
            return new HtmlParser(info).Parse(htmlPortion);
        }

        public IEnumerable<INode> Parse(HtmlPortion portion,IHtmlParser[] parsers)
        {

            Func<HtmlPortion, IHtmlParser[], INode> fn = (p, pa) =>
            {
                foreach (var parser in parsers)
                {
                    if (portion.IsEnd) return null;
                    var result = parser.Parse(this, portion);
                    if (result == null) continue;
                    return result;
                }
                return null;
            };
            while (portion.HasNext)
            {
                if (portion.IsWhiteSpace)
                {
                    portion.Next();
                    continue;
                }
                var result = fn(portion, parsers);
                if (result != null)
                { 
                    yield return result;
                    if(result is IParseBreak)
                        yield break;
                }
                else portion.Jump();
            }

        }
        public IEnumerable<INode> Parse(HtmlPortion portion)
        {
            return Parse(portion,
                new IHtmlParser[]
                    {new CommentParser(this.Info),
                     new ScriptParser(this.Info),
                     new StyleParser(this.Info),
                     new TagParser(this.Info),
                     new EndTagParser(),
                     new TextParser(this.Info)});
        }

        public AttributeParseResult GetAttribute(HtmlPortion portion)
        {
            return new AttributesParser().Parse(portion);
        }



        public string EscapeString(string pattern)
        {
            var regex = new Regex(@"(""|')((?:\\\1|(?:(?!\1).))*)\1");
            var matches = regex.Matches(pattern);
            foreach (Match match in matches)
                pattern = pattern.Replace(match.Value, "@@es_"+Guid.NewGuid().ToString("N"));
            
            return pattern;
        }

    

    }
}
