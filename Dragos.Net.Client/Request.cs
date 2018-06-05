using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Dragos.Net.Client.DataProviders;
using System.Collections.Generic;
using Dragos.Net.Client.DataProviders.XmlRpc;

namespace Dragos.Net.Client
{
    
    public abstract class BaseRequest :IRequest
    {
        private HeaderCollection HeadersCollection { get; } = new HeaderCollection();

        public IEnumerable<Header> Headers
        {
            get { return HeadersCollection.ToArray(); }
        }
        public CookieCollection Cookies { get; private set; } = new CookieCollection();

        public Uri ReferrerUri { get; private set; }

        public Encoding WriteEncoding { get; set; } = Encoding.UTF8;

        public Encoding ReadEncoding { get; set; } = Encoding.UTF8;

        private readonly IList<IRequestObserver> _observer = new List<IRequestObserver>();

        public bool Ssl { get; set; }

        public int Timeout { get; set; } = 100000;

        public bool PreAuthenticate { get; set; } = false;

        public Uri Url { get; }

        public bool AllowAutoRedirect { get; set; }


        public ContentProviderCollection ContentProviders { get; } = new ContentProviderCollection();

        public bool KeepAlive { get; set; }

        public WebClient Client { get; private set; }

        internal BaseRequest(WebClient client,Uri url,params IRequestObserver[] observers)
        {
            this.Url = url;
            this.Client = client;
            this.AddHeader("content-type", "application/x-www-form-urlencoded");

            this.AddObserver(observers);
        }

        public Response GetResponse(RequestMethod method, object data)
        {
            Response result = null;
            try
            {
                var rdata = data;
                var rtype = method;
                this.CallObserver(ob => ob.OnRequest(new RequestStartArg(this,Url, rdata, rtype)));
                var request = WebRequest.CreateHttp(Url);
                request.Method = method.ToString().ToUpper();
                request.Timeout = Timeout;
                AddHeader(request);
                request.PreAuthenticate = PreAuthenticate;
                if (Ssl)
                    request.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
                request.CookieContainer = new CookieContainer();
                foreach (var cookie in this.Cookies)
                    request.CookieContainer.Add(Url, new System.Net.Cookie(cookie.Name, cookie.Value));
                if (!IsNonBodyMethod(method))
                {
                    var stream = request.GetRequestStream();
                    var bytes = GetBytes(data);
                    stream.Write(bytes, 0, bytes.Length);
                }
                result = GetWebResponse((HttpWebResponse)request.GetResponse());
                ReferrerUri = result.Uri;
                var resultmedator = new ResponseMediator(result);
                this.CallObserver(
                    observer =>
                        observer.OnResponse(new RequestEndArg(resultmedator, this, Url, rdata, rtype)));
                return resultmedator.Response;

            }
            catch (WebException exception)
            {
                result = GetWebResponse((HttpWebResponse) exception.Response);
                var responseMediator = new ResponseMediator(result);
                this.CallObserver(observer => observer.OnException(new RequestException(exception, this,responseMediator, Url, data, method)));
                return responseMediator.Response;
            }
        }

        private RequestMethod[] GetNonBodyMethods()
        {
            return new [] { RequestMethod.Get, RequestMethod.Head };
        }

        private bool IsNonBodyMethod(RequestMethod method)
        {
            return GetNonBodyMethods().Contains(method);
        }

        private Response GetWebResponse(HttpWebResponse response)
        {
            if (response == null) return null;
            using (response)
            {
                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream == null)
                        return CreateResponse(response, null);
                        var result = CreateResponse(response, responseStream);
                        return result;
                }
            }
        }



        private byte[] GetBytes(object data)
        {
            if (data == null) return new byte[0];
            return WriteEncoding.GetBytes(GetValue(data));
        }

        private Response CreateResponse(HttpWebResponse response, Stream responseContent)
        {
            var headers = (from string h in response.Headers select new Header(h, response.Headers[h])).ToList();
            var responseCookies = new CookieCollection();
            foreach (System.Net.Cookie cookie in response.Cookies)
                responseCookies.AddOrUpdate(cookie.Name, cookie.Value);
            return new Response(this.Client,response.ResponseUri.AbsoluteUri, (int) response.StatusCode, headers, responseContent, this,
                responseCookies, this.ContentProviders);
        }

        public void AddObserver(IRequestObserver observer)
        {
            this._observer.Add(observer);
        }

        private void AddObserver(IRequestObserver[] observers)
        {
            foreach (var observer in observers)
                this.AddObserver(observer);
        }

        private void CallObserver(Action<IRequestObserver> action)
        {
            foreach (var observer in this._observer)
                try
                {
                    action.Invoke(observer);
                }
                catch (Exception ex)
                { //ignored}
                }
        }

        private IDataProvider GetProvider()
        {
            var firstOrDefault = ContentProviders.Get(HeadersCollection["content-type"]);
            if (firstOrDefault == null) return new TextDataProvider();
            return firstOrDefault;
        }

        private string GetValue(object data)
        {
            return GetProvider().GetValue(data) as string;
        }

        private void AddHeader(HttpWebRequest request)
        {
            foreach (var header in Headers)
            {
                if (header.Name == "content-type")
                    request.ContentType = header.Value;
                else if (header.Name == "user-agent")
                    request.UserAgent = header.Value;
                else if (header.Name == "connection" && header.Value == "keep-alive")
                {
                    request.KeepAlive = true;
                    request.ServicePoint.Expect100Continue = false;
                }
                else if (header.Name == "referer")
                    request.Referer = header.Value;
                else if (header.Name == "accept")
                    request.Accept = header.Value;

                else request.Headers.Add(GetHeaderName(header.Name), header.Value);
            }
        }

        private string GetHeaderName(string name)
        {
            var result = string.Empty;
            for(var i=0;i<name.Length;i++)
            {
                if (i == 0)
                    result += char.ToUpper(name[i]);
                else if (name[i - 1] == '-')
                    result += char.ToUpper(name[i]);
                else result += char.ToLower(name[i]);
            }
            return result;
        }


        public IRequest AddHeader(string name, string value)
        {
            this.HeadersCollection.Add(name, value);
            return this;
        }

        public IRequest RemoveHeader(string name)
        {
            this.HeadersCollection.Remove(name);
            return this;
        }


    }

    public class Request : BaseRequest
    {
        private IDataProvider _xml;

        public IDataProvider Xml
        {
            get { return _xml; }
            set
            {
                _xml = value;
                ContentProviders.Add("application/xml", value);
                ContentProviders.Add("text/xml", value);
            }
        }

        private IDataProvider _html;
        public IDataProvider Html
        {
            get { return _html; }
            set
            {
                _html = value;
                ContentProviders.Add("text/html", value);
            }
        }
        private IDataProvider _webform;
        public IDataProvider WebForm
        {
            get
            {
                return _webform;
            }
            set
            {
                _webform = value;
                ContentProviders.Add("application/x-www-form-urlencoded", value);
            }
        }

        public Request(WebClient client,string url):base(client, new Uri(url))
        {

            this.Xml = new XmlRpcDataProvider();
            this.Html = DataProvider.Html(new Client.Html.DocInfo(Client,this.Url));
            this.WebForm = new WebFormDataProvider();
        }

    
    }

}
