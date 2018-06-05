namespace Dragos.Net.Client.Html.Tags
{
    public class Form : PairTag
    {
        public string Enctype
        {
            get
            {
                if (HasEnctype)
                    return this.Attributes["enctype"];
                return this._defaultEnctype;
            }
        }

        public bool HasEnctype
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.Attributes["enctype"]);
            }
        }

        private string _defaultEnctype = "application/x-www-form-urlencoded";

        public string Method
        {
            get
            {
                return this.Attributes["method"];
            }
        }

        public string Action
        {
            get
            {
                return this.Attributes["action"];
            }
        }

        public Form(string tagName, IAttributes attributes, DocInfo info) : base(tagName, attributes,info)
        {
            
        }

        public Response Submit(System.Action<Parameter> parameterSelector)
        {
            var parameter = this.Elements<IEntry>().ToParameter();
            parameterSelector.Invoke(parameter);
            var request = DocInfo.Client
                  .GetRequest(GetAction(parameter));
            request.AddHeader("content-type", Enctype);
            var method = GetMethod(Method);
            return request.GetResponse(method, parameter.ToObject());
        }


        private string GetAction(Parameter parameter)
        {
            var r = IsOwnUrl(Action) ? DocInfo.Uri.AbsoluteUri : Action;
            if (Method.ToLower() == "get")
                return  GetUrlNonQuery(r)+ "?" + parameter.ToQueryString();
            return r;
        }

        private string GetUrlNonQuery(string url)
        {
            if (url.Contains("?"))
                return url.Split('?')[0];
            return url;

        }
    
       


        public Response Submit()
        {
            return Submit(action => { });
        }

        private RequestMethod GetMethod(string method)
        {
            return (RequestMethod)System.Enum.Parse(typeof(RequestMethod),MethodCamelCase(method));
        }

        private string MethodCamelCase(string method)
        {
            var result = string.Empty;
            for (var i = 0; i < method.Length; i++)
                if (i == 0)
                    result += char.ToUpper(method[i]);
                else result += char.ToLower(method[i]);
            return result;
        }

        private static bool IsOwnUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return true;
            if (url.Contains("/"))
                return string.IsNullOrWhiteSpace(url.Split('/')[1]);
            return true;
        }
    }
}
