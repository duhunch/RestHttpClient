using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Dragos.Data.Imitation;
using Dragos.Net.Client.Html;

namespace Dragos.Net.Client
{

    public enum ResponseStatus
    {
        Null=-1,
        Continue = 100,
        SwitchingProtocols = 101,
        Processing = 102,
        Ok = 200,
        Created = 201,
        Accepted = 202,
        NonAuthoritative=203,
        NoContent = 204,
        ResetContent = 205,
        PartialContent = 206,
        MultiStatus = 207,
        AlreadyReported = 208,
        ImUsed = 226,
        MultipleChoices = 300,
        MovedPermanently = 301,
        Found = 302,
        SeeOther = 303,
        NotModified = 304,
        UseProxy = 305,
        TemporaryRedirect = 307,
        PermanentRedirect = 308,
        BadRequest = 400,
        Unauthorized = 401,
        PaymentRequired = 402,
        Forbidden = 403,
        NotFound = 404,
        MethodNotAllowed = 405,
        NotAcceptable = 406,
        ProxyAuthenticationRequired = 407,
        RequestTimeout = 408,
        Conflict = 409,
        Gone = 410,
        LengthRequired = 411,
        PreconditionFailed = 412,
        PayloadTooLarge = 413,
        RequestUriTooLong = 414,
        UnsupportedMediaType = 415,
        RequestedRangeNotSatisfiable = 416,
        ExpectationFailed = 417,
        ImATeapot = 418,
        MisdirectedRequest = 421,
        Locked = 423,
        FailedDependency = 424,
        UpgradeRequired = 426,
        TooManyRequests = 429,
        RequestHeaderFiledsTooLarge = 431,
        ConnectionClosedWithoutResponse = 444,
        UnavailableForLegalReasons = 451,
        ClientClosedRequest = 499,
        InternalServerError = 500,
        NotImplemented = 501,
        BadGateway = 502,
        ServiceUnavailable = 503,
        GatewayTimeout = 504,
        HttpVersionNotSupported = 505,
        VariantAlsoNegotiates = 506,
        InsufficientStorage = 507,
        LoopDetected = 508,
        NotExtended = 510,
        NetworkAuthenticationRequired = 511,
        Timeout = 599
    }

    public class ResponseHeader
    {
        public string Name { get; }
        public string Value { get; }
        public ResponseHeader(string name, string value)
        {
            Name = name;
            Value = value;
        }

    }

    public class Response
    {
        public Uri Uri { get; }
        public string ContentText { get; }
        public WebClient Client { get; }
        public IRequest Request { get; }

        public ContentProviderCollection Providers { get; }

        public dynamic Data
        {
            get
            {
                var contentType = this.Headers["content-type"];
                if (contentType == null) return null;
                if (this.Providers.Has(contentType))
                    return Providers[contentType].TryParse(ContentText);
                return null;
            }
        }

        public ResponseStatus Status
        {
            get
            {
                if (Enum.GetValues(typeof(ResponseStatus)).Cast<int>().Any(x => x == StatusCode))
                    return (ResponseStatus) StatusCode;
                return ResponseStatus.Null;
            }
        }



        public int StatusCode { get; private set; }


        public HeaderCollection Headers { get; private set; }

        public CookieCollection Cookies { get; private set; }

        public bool IsSuccess => IsCodeRange(200, 300);

        public bool IsClientError => IsCodeRange(400, 500);

        public bool IsServerError => IsCodeRange(500, 600);

        public bool IsRedirected => IsCodeRange(300, 500);

        public bool IsInformational => IsCodeRange(100, 200);

        private bool IsCodeRange(int start, int end)
        {
            return Convert.ToInt32(StatusCode) >= start && Convert.ToInt32(StatusCode) < end;
        }

        public Response(WebClient client,string url, int statusCode, IEnumerable<Header> headers, Stream content, IRequest request, CookieCollection cookies,ContentProviderCollection providers)
        {
            this.Client = client;
            this.Uri = new Uri(url);
            this.Headers = new HeaderCollection();
            foreach (var header in headers)
                this.Headers.Add(header.Name, header.Value);
            this.StatusCode = statusCode;
            this.ContentText = GetText(content);
            this.Request = request;
            this.Cookies = cookies;
            this.Providers = providers;
        }

        private string GetText(Stream stream)
        {
            using (var sr = new StreamReader(stream, GetEncoding()))
                return sr.ReadToEnd();
        }

        private Encoding GetEncoding()
        {
            var contentType = Headers["content-type"];
            var charset = contentType?.Split(';').FirstOrDefault(x => x.Trim().StartsWith("charset",StringComparison.OrdinalIgnoreCase));
            if(charset == null) return Encoding.UTF8;
            if (!charset.Contains("=")) return Encoding.UTF8;
            var encoding = charset.Split('=')[1];
            return Encoding.GetEncoding(encoding);
        }

        public TModel GeData<TModel>()
        {
            return GetData<TModel>(Data);
        }

        public static TModel GetData<TModel>(object content)
        {
            var r = GetData(typeof(TModel),content);
            return (TModel) r;
        }

        public static object GetData(Type type, object content)
        {
            return new DataImitateProvider().Imitate(type, content);
        }
    }
}
