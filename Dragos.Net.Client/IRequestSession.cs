
using System;
using System.Collections.Generic;

namespace Dragos.Net.Client
{
    public interface  IRequest
    {
        WebClient Client { get; }
        Uri Url { get; }
        IEnumerable<Header> Headers { get; }
        CookieCollection Cookies { get; }
        int Timeout { get; set; }
        IRequest AddHeader(string name, string value);
        IRequest RemoveHeader(string name);
        Response GetResponse(RequestMethod method,object data);
        void AddObserver(IRequestObserver observer);
    }
}
