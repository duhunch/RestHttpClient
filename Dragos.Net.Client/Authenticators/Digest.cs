using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Dragos.Net.Client.Authenticators
{
    public interface IDigestHashAlgorithm
    {
        string Name { get; }
        string Compute(string str);
    }

    public class DigestMd5 : IDigestHashAlgorithm
    {
        public string Name { get; } = "MD5";

        private readonly Encoding _encoding = Encoding.UTF8;

        public DigestMd5(Encoding encoding)
        {
            this._encoding = encoding;
        }
        public DigestMd5()
        {
        }
        public string Compute(string str)
        {
            var inputBytes = _encoding.GetBytes(str);
            var hash = MD5.Create().ComputeHash(inputBytes);
            var sb = new StringBuilder();
            foreach (var b in hash)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }

    public class Digest : IAuthenticator, IRequestObserver
    {
        private string _user;
        private string _password;
        private string _realm;
        private string _nonce;
        private string _qop;
        private string _cnonce;
        private string _algorithm
        {
            get { return DigestHash.Name; }
        }
        private string _opaque;
        private DateTime _cnonceDate;
        private int _nc;
        private int _times = 0;
        private IDigestHashAlgorithm DigestHash { get; }
        public string CredentialName => "Digest";

        public Digest(string username, string password, IDigestHashAlgorithm digestHash)
        {
            this._user = username;
            this._password = password;
            DigestHash = digestHash;

        }

        public Digest(string username, string password)
        {
            this._user = username;
            this._password = password;
            DigestHash = new DigestMd5();
        }

        public void Apply(WebClient client)
        {
            client.AddObserver(this);
        }

        public void OnResponse(RequestEndArg arg)
        {
            _times = 0;
        }

        private void GetWwwAuthenticate(Response response)
        {
            var wwwAuthenticateHeader = response.Headers["WWW-Authenticate"];
            _realm = GetDigestHeaderAttribute("realm", wwwAuthenticateHeader);
            _nonce = GetDigestHeaderAttribute("nonce", wwwAuthenticateHeader);
            _qop = GetDigestHeaderAttribute("qop", wwwAuthenticateHeader);
            _opaque = GetDigestHeaderAttribute("opaque", wwwAuthenticateHeader);
            _cnonce = new Random().Next(123400, 9999999).ToString();
            _cnonceDate = DateTime.Now;
        }

        private string GetDigestHeaderAttribute(string attributeName, string digestAuthHeader)
        {
            var regHeader = new Regex($@"{attributeName}=""([^""]*)""");
            var matchHeader = regHeader.Match(digestAuthHeader);
            if (matchHeader.Success)
                return matchHeader.Groups[1].Value;
            else return null;
            //throw new ApplicationException(string.Format("Header {0} not found", attributeName));
        }


        public void OnRequest(RequestStartArg arg)
        {
            //arg.Request.PreAuthenticate = true;
            if (!string.IsNullOrEmpty(_cnonce) &&
                               DateTime.Now.Subtract(_cnonceDate).TotalHours < 1.0)
            {
                arg.Request.AddHeader("Authorization", ComputeDigestHeader(arg.Url, arg.RequestType.ToString().ToUpper()));
            }

        }

        private string ComputeDigestHeader(Uri uri, string method)
        {
            _nc = _nc + 1;
            return ComputeDigestHeader(uri, _user, method, _realm, _password, _cnonce, _qop, _nonce, _nc, _opaque,
                _algorithm);
        }

        private string ComputeDigestHeader(Uri uri, string user,string method, string realm, string password, string cnonce, string qop,
            string nonce, int nc, string opaque, string algorithm)
        {
            var ha1 = DigestHash.Compute($"{user}:{realm}:{password}");
            var ha2 = DigestHash.Compute($"{method}:{uri.PathAndQuery}");
            var raw = $"{ha1}:{_nonce}:{_nc:00000000}:{cnonce}:{qop}:{ha2}";
            var digestResponse = DigestHash.Compute(raw);

            return string.Format("Digest username=\"{0}\", " +
                                 "realm=\"{1}\", nonce=\"{2}\", " +
                                 "uri=\"{3}\", " +
                                 "algorithm={9}, " +
                                 "qop={5}, " +
                                 "nc={6:00000000}, " +
                                 "cnonce=\"{7}\", " +
                                 "response=\"{4}\", " +
                                 "opaque=\"{8}\"",
                user, realm, nonce, uri.PathAndQuery, digestResponse, qop, nc, cnonce,opaque, _algorithm);
        }



        public void OnException(RequestException arg)
        {
            if (_times > 0) return;
            if (arg.Response.Status == ResponseStatus.Unauthorized)
            {
                _nc = 0;
                _times++;
                GetWwwAuthenticate(arg.Response);
                var newResponse = arg.Request.Client.GetRequest(arg.Url.AbsoluteUri).GetResponse(arg.RequestType, arg.ContentData);
                arg.Manipulate(newResponse);
            }
            else
            {
                _times = 0;
            }
        }
    }
}
