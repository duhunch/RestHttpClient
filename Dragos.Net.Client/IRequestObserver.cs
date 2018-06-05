using System;

namespace Dragos.Net.Client
{
    public class ResponseMediator
    {
        public Response Response { get; private set; }
        public ResponseMediator(Response response)
        {
            this.Response = response;
        }

        public void SetResponse(Response response)
        {
            Response = response;
        }
    }
    public class RequestException
    {
        public Exception Exception { get; }

        public Response Response
        {
            get
            {
                return _mediator.Response;
            }
        }
        private ResponseMediator _mediator;
        public IRequest Request { get; }
        public Uri Url { get; }
        public object ContentData { get; }
        public RequestMethod RequestType { get; }
        public RequestException(Exception ex,IRequest client,ResponseMediator response, Uri url, object contentData, RequestMethod requestType)
        {
            this.Request = client;
            this.Url = url;
            this.ContentData = contentData;
            this.RequestType = requestType;
            this.Exception = ex;
            this._mediator = response;
        }
        public void Manipulate(Response response)
        {
            this._mediator.SetResponse(response);
        }
    }


    public class RequestStartArg
    {
        public IRequest Request { get; }
        public Uri Url { get; }
        public object ContentData { get; }
        public RequestMethod RequestType { get; }
        public RequestStartArg(IRequest client, Uri url, object contentData, RequestMethod requestType)
        {
            this.Request = client;
            this.Url = url;
            this.ContentData = contentData;
            this.RequestType = requestType;
        }

    }
    public class RequestEndArg
    {
        public Response Response { get {
                return _mediator.Response;
            } }
        public IRequest Request { get; }
        public Uri Url { get;  }
        public object ContentData { get; }
        public RequestMethod RequestType { get; }
        private ResponseMediator _mediator;
        internal RequestEndArg(ResponseMediator mediator,IRequest client,Uri url,object contentData,RequestMethod requestType)
        {
            this.Request = client;
            this.Url = url;
            this.ContentData = contentData;
            this.RequestType = requestType;
            _mediator = mediator;
        }

        public void Manipulate(Response response)
        {
            this._mediator.SetResponse(response);
        }
    }
    public interface IRequestObserver
    {
        void OnRequest(RequestStartArg arg);
        void OnResponse(RequestEndArg arg);
        void OnException(RequestException arg);
    }
}
