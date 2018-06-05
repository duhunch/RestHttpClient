using System;


namespace Dragos.Net.Client.Authenticators
{
    public class Basic : IAuthenticator
    {
        public string CredentialName => "Basic";

        private string _username;
        private string _password;
        public Basic(string username,string password)
        {
            this._username = username;
            this._password = password;
        }

        public void Apply(WebClient request)
        {
            request.AddObserver(this);
        }

        public void OnRequest(RequestStartArg arg)
        {
            String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(_username + ":" + _password));
            arg.Request.AddHeader("Authorization", "Basic " + encoded);
        }

        public void OnResponse(RequestEndArg arg)
        {
       
        }

        public void OnException(RequestException arg)
        {
        }
    }
}
