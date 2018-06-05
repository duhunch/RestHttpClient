using Dragos.Net.Client.Html;

namespace Dragos.Net.Client
{
    public static class Extentions
    {
        public static Response Get(this IRequest request)
        {
            return request.GetResponse(RequestMethod.Get, null);
        }

        public static Response Post(this IRequest request, object data = null)
        {
            return request.GetResponse(RequestMethod.Post, data);
        }

        public static Response Post(this IRequest request, Parameter parameter)
        {
            return request.Post(parameter.ToObject());
        }
    }
}
