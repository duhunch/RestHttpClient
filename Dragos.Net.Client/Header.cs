using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Dragos.Net.Client
{
    public class Header
    {
        public string Name { get; }
        public string Value { get; }
        public Header(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }

    public static class HeaderType
    {
        /// <summary>
        /// Defines the authentication method that should be used to gain access to a resource.
        /// </summary>
        public static string WwwAuthenticate { get; } = "WWW-Authenticate";
        /// <summary>
        /// Contains the credentials to authenticate a user agent with a server.
        /// </summary>
        public static string Authorization { get; } = "Authorization";
        /// <summary>
        /// Defines the authentication method that should be used to gain access to a resource behind a Proxy server.
        /// </summary>
        public static string ProxyAuthenticate { get; } = "Proxy-Authenticate";
        /// <summary>
        /// Contains the credentials to authenticate a user agent with a proxy server.
        /// </summary>
        public static string ProxyAuthorization { get; } = "Proxy-Authorization";
        /// <summary>
        /// The time in seconds the object has been in a proxy cache.
        /// </summary>
        public static string Age { get; } = "Age";
        /// <summary>
        /// Specifies directives for caching mechanisms in both requests and responses.
        /// </summary>
        public static string CacheControl { get; } = "Cache-Control";
        /// <summary>
        /// The date/time after which the response is considered stale.
        /// </summary>
        public static string Expires { get; } = "Expires";
        /// <summary>
        /// Implementation-specific header that may have various effects anywhere along the request-response chain. Used for backwards compatibility with HTTP/1.0 caches where the Cache-Control header is not yet present.
        /// </summary>
        public static string Pragma { get; } = "Pragma";
        /// <summary>
        /// A general warning field containing information about possible problems.
        /// </summary>
        public static string Warning { get; } = "Warning";
        /// <summary>
        /// Informs the server about the types of data that can be sent back. It is MIME-type.
        /// </summary>
        public static string Accept { get; } = "Accept";
        /// <summary>
        /// Informs the server about which character set the client is able to understand.
        /// </summary>
        public static string AcceptCharset { get; } = "Accept-Charset";
        /// <summary>
        /// Informs the server about the encoding algorithm, usually a compression algorithm, that can be used on the resource sent back.
        /// </summary>
        public static string AcceptEncoding { get; } = "Accept-Encoding";
        /// <summary>
        /// Informs the server about the language the server is expected to send back. This is a hint and is not necessarily under the full control of the user: the server should always pay attention not to override an explicit user choice (like selecting a language in a drop down list).
        /// </summary>
        public static string AcceptLanguage { get; } = "Accept-Language";
        /// <summary>
        /// Is a response header if the resource transmitted should be displayed inline (default behavior when the header is not present), or it should be handled like a download and the browser should present a 'Save As' window.
        /// </summary>
        public static string ContentDisposition { get; } = "Content-Disposition";
        /// <summary>
        /// indicates the size of the entity-body, in decimal number of octets, sent to the recipient.
        /// </summary>
        public static string ContentLength { get; } = "Content-Length";
        /// <summary>
        /// Indicates the media type of the resource.
        /// </summary>
        public static string ContentType { get; } = "Content-Type";
        /// <summary>
        /// Used to specify the compression algorithm.
        /// </summary>
        public static string ContentEncoding { get; } = "Content-Encoding";
        /// <summary>
        /// Describes the language(s) intended for the audience, so that it allows a user to differentiate according to the users' own preferred language.
        /// </summary>
        public static string ContentLanguage { get; } = "Content-Language";
        /// <summary>
        /// Indicates an alternate location for the returned data.
        /// </summary>
        public static string ContentLocation { get; } = "Content-Location";
        /// <summary>
        /// Specifies the domain name of the server (for virtual hosting), and (optionally) the TCP port number on which the server is listening.
        /// </summary>
        public static string Host { get; } = "Host";
        /// <summary>
        /// Contains an Internet email address for a human user who controls the requesting user agent.
        /// </summary>
        public static string From { get; } = "From";
        /// <summary>
        /// The address of the previous web page from which a link to the currently requested page was followed.
        /// </summary>
        public static string Referer { get; } = "Referer";
        /// <summary>
        /// Governs which referrer information sent in the Referer header should be included with requests made.
        /// </summary>
        public static string ReferrerPolicy { get; } = "Referrer-Policy";
        /// <summary>
        /// Contains a characteristic string that allows the network protocol peers to identify the application type, operating system, software vendor or software version of the requesting software user agent
        /// </summary>
        public static string UserAgent { get; } = "User-Agent";
    }


    public class HeaderCollection : IEnumerable<Header>
    {
        private readonly List<Header> _headers = new List<Header>();

        public HeaderCollection Add(string name, string value)
        {
            if (name == null) return this;
            if (!Has(name))
                this._headers.Add(new Header(name.ToLower(), value));
            else this.Replace(name.ToLower(), value);
            return this;
        }
        

        public bool Remove(string name)
        {
            var item =
                _headers.FirstOrDefault(x => In(x.Name,name));
            if (item == null) return false;
            _headers.Remove(item);
            return true;
        }


        public bool Has(string name)
        {
            return _headers.Any(x =>In(x.Name,name));
        }

        public string Get(string name)
        {
            var item = _headers.FirstOrDefault(x => In(x.Name,name));
            return item?.Value;
        }

        private bool In(string headerName,string value)
        {
            if (value == null) return false;
            return headerName.Split(';').Any(a => a == value.ToLower());
        }

        public void Replace(string name, string value)
        {
           var item =  _headers.First(x => In(x.Name,name.ToLower()));
           var index = _headers.IndexOf(item);
            this._headers[index] = new Header(name, value);
        }


        public IEnumerator<Header> GetEnumerator()
        {
            return _headers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public string this[string name]
        {
            get { return Get(name); }
            set { this.Add(name, value); }
        }
    }
}
