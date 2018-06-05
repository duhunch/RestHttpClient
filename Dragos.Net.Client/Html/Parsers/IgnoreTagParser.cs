using System.Text.RegularExpressions;
using Dragos.Net.Client.Html.Tags;

namespace Dragos.Net.Client.Html.Parsers
{
    public class EndTagParser :IHtmlParser
    {
        public INode Parse(HtmlParser parser, HtmlPortion current)
        {
            if (new Regex(@"</[a-zA-Z]").IsMatch(current.GetText(3)))
            {
                return new IgnoreElement();
            }
            return null;
        }
    }
}
