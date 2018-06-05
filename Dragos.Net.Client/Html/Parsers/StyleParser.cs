using System;
using System.Text.RegularExpressions;
using Dragos.Net.Client.Html.Tags;

namespace Dragos.Net.Client.Html.Parsers
{
    public class StyleParser :IHtmlParser
    {

        private DocInfo _info;
        public StyleParser(DocInfo info)
        {
            this._info = info;
        }

        public INode Parse(HtmlParser parser, HtmlPortion current)
        {
            if (!IsStyle(current)) return null;

            var attr = parser.GetAttribute(current);
            var end = current.StartIndexOf(new Regex(@"</style\s*>"));
            var contentIndex = end - current.Current;
            current.Next();
            var content = current.Substring(contentIndex);
            current.Jump(contentIndex);
            if (attr.IsSingle) return new StyleTag("<style", attr.Attributes, "", _info);
            var result = new StyleTag("style", attr.Attributes, content, _info);
            current.Jump(current.IndexOf(new Regex(@"</style\s*>")));
            current.Jump();
            return result;
        }

        private static bool IsStyle(HtmlPortion current)
        {
            //var with = current.NextWith;
            //if (with.Contains("scri") && !current.Is("script") && current.Current > 60000)
            //{
            //    var seven = current.GetText(7);
            //    var result2 = current.Is("<script");
            //}
            var result = current.Is("<style");
            return result;
        }

    }
}
