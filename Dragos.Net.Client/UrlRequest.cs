using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dragos.Net.Client
{
    public class UrlRequest 
    {
        private readonly string _host;

        public string Url
        {
            get
            {
                return  _host +_urlSegments.Aggregate(_host, (current, url) => new Regex(@"{\s*" + url.Key + @"\s*}").Replace(current, url.Value));
            }
        }

        private readonly List<KeyValuePair<string, string>> _urlSegments = new List<KeyValuePair<string, string>>();

        public UrlRequest(string url)
        {
            _host = url;
        }

        public UrlRequest Anchor(string key, string value)
        {
            this._urlSegments.Add(new KeyValuePair<string, string>());
            return this;
        }
    }
}
