
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;

namespace Dragos.Net.Client
{
    public class WebClient:IRequestObserver
    {


        public bool Ssl { get; set; }

        public int Timeout { get; set; } = 100000;

        public bool AllowAutoRedirect { get; set; } = false;

        public IAuthenticator Authenticator { get; set; }

        private readonly IList<IRequestObserver> _observer = new List<IRequestObserver>();

        public CookieCollection Cookies { get; private set; } = new CookieCollection();

        public string ContentType { get; set; } = "application/x-www-form-urlencoded";

        private HeaderCollection _initHeaders = new HeaderCollection();

        public string Host { get; private set; }

       

        public WebClient(string host)
        {
            Host = host;
         
        }

        public WebClient Clone(string host)
        {
            var result = new WebClient(host);
            foreach (var cookie in this.Cookies)
                result.Cookies.AddOrUpdate(cookie.Name, cookie.Value);
            foreach (var header in this._initHeaders)
                result._initHeaders.Add(header.Name, header.Value);
            result.AllowAutoRedirect = this.AllowAutoRedirect;
            result.Ssl = this.Ssl;
            result.KeepAlive();
            return result;

        }

        private static void CallSsl()
        {
            ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, errors) => true);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        }


        public Request GetRequest(string url = null)
        {
            var request = new Request(this,GetUrl(url));
            request.Ssl = this.Ssl;
            SetRequest(request);
            return request;
        }

        public WebClient Header(string name,string value)
        {
            this._initHeaders.Add(name, value);
            return this;
        }

        public WebClient SetContentType(string content)
        {
            ContentType = content;
            return this;
        }

        public WebClient KeepAlive()
        {
            this.Header("connection", "keep-alive");
            return this;
        }

        private void SetRequest(IRequest request)
        {
            Authenticator?.Apply(this);
            request.Timeout = Timeout;
            if (AllowAutoRedirect)
                request.AddObserver(new AutoRedictObserver());
            request.AddHeader("content-type", ContentType);
            foreach (var header in _initHeaders)
                request.AddHeader(header.Name, header.Value);
            request.AddObserver(this);
            foreach (var observer in _observer)
                request.AddObserver(observer);
            foreach (var cookie in Cookies)
                request.Cookies.AddOrUpdate(cookie.Name, cookie.Value);
        }

        public WebClient SslEnable()
        {
            this.Ssl = true;
            return this;
        }

        public WebClient AutoRedirectEnable()
        {
            this.AllowAutoRedirect = true;
            return this;
        }

        public WebClient UserAgent(string value)
        {
            this.Header("User-Agent", value);
            return this;
        }

        public WebClient SetTimeout(int value)
        {
            Timeout = value;
            return this;
        }

        public WebClient Authenticate(IAuthenticator authenticator)
        {
            this.Authenticator = authenticator;
            return this;
        }


        public TRequest GetRequest<TRequest>(string url = null) where TRequest : IRequest
        {
            var result = (TRequest) Activator.CreateInstance(typeof(TRequest), new object[] {this, GetUrl(url)});
            SetRequest(request:result);
            return result;
        }

        private bool IsAbsoluteUrl(string str)
        {
            return System.Uri.IsWellFormedUriString(str, UriKind.Absolute);
        }

        private string GetUrl(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return Host;
            if (IsAbsoluteUrl(str))
                return str;
            var host = Host;
            var u = str.First() == '/' ? str.Substring(1, str.Length - 1) : str;
            return host + "/" + u;
        }

        public TRequest GetRequest<TRequest>(UrlRequest url) where TRequest : IRequest
        {
            return GetRequest<TRequest>(url.Url);
        }

        public IRequest GetRequest(UrlRequest url)
        {
            return GetRequest(url.Url);
        }

        public WebClient AddObserver(IRequestObserver observer)
        {
            this._observer.Add(observer);
            return this;
        }

    

        public void OnRequest(RequestStartArg arg)
        {
            if(Ssl)
                CallSsl();
        }

        public void OnResponse(RequestEndArg arg)
        {
     
            foreach (var cookie in arg.Response.Cookies)
                this.Cookies.AddOrUpdate(cookie.Name, cookie.Value);
        }

        public void OnException(RequestException arg)
        {
          
        }
    }

    internal class AutoRedictObserver : IRequestObserver
    {
        public void OnRequest(RequestStartArg arg)
        {
          
        }

        public void OnResponse(RequestEndArg arg)
        {
            if (arg.Response.Headers.Any(x=>x.Name.Equals("location",StringComparison.OrdinalIgnoreCase)))
            {
                var location = arg.Response.Headers["location"];
                if(System.Uri.IsWellFormedUriString(location,UriKind.Absolute))
                {
                    if(arg.Url.Host != new System.Uri(location).Host)
                    {
                        var request = arg.Request.Client.GetRequest(location);
                        request.AddHeader("referer", arg.Request.Url.Host);
                        arg.Manipulate(request.GetResponse(RequestMethod.Get, null));
                        return;
                    }
                }
                arg.Manipulate(arg.Request.Client.GetRequest(location).GetResponse(RequestMethod.Get,null));
            }
        }

        public void OnException(RequestException arg)
        {
            
        }
    }
}
