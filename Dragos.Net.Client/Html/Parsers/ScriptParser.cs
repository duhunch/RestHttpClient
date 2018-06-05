using System;
using System.Text.RegularExpressions;
using Dragos.Net.Client.Html.Tags;

namespace Dragos.Net.Client.Html.Parsers
{
    public class ScriptParser :IHtmlParser
    {

        private DocInfo _info;
        public ScriptParser(DocInfo info)
        {
            this._info = info;
        }

        public INode Parse(HtmlParser parser, HtmlPortion current)
        {
            if (!IsScript(current)) return null;

            var attr = parser.GetAttribute(current);
            var end = current.StartIndexOf(new Regex(@"</script\s*>"));
            var contentIndex = end - current.Current;
            current.Next();
            var content = current.Substring(contentIndex);
            current.Jump(contentIndex);
            if (attr.IsSingle) return new ScriptTag("script", attr.Attributes, "",_info);
            var result = new ScriptTag("script", attr.Attributes, content,_info);
            current.Jump(current.IndexOf(new Regex(@"</script\s*>")));
            current.Jump();
            return result;
        }

        private static bool IsScript(HtmlPortion current)
        {
            var result = current.Is("<script");
            return result;
        }

       
    }
}
