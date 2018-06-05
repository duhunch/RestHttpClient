using System;
using Dragos.Net.Client.Html;

namespace Dragos.Net.Client.DataProviders
{
    public class HtmlObjectProvider :IDataProvider
    {
        public DocInfo Info { get; private set; }
        public HtmlObjectProvider(DocInfo info)
        {
            Info = info;
        }

        public string GetValue(object value)
        {
            return value as string;
        }

        public object TryParse(string value)
        {
            return new HtmlParser(Info).Parse(value);
        }
    }
}
