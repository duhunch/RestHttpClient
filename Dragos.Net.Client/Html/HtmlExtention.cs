
using Dragos.Net.Client.Html.Tags;
using System.Collections.Generic;
using System.Linq;

namespace Dragos.Net.Client.Html
{
    public static class HtmlExtentions
    {
       public static dynamic ToObject(this IEnumerable<IEntry> source)
        {
            dynamic result = new HtmlObjectDynamic(source.Select(x=> new KeyValuePair<string, object>(x.Name,x.Value)).ToArray());
            return result;
        }
        public static Parameter ToParameter(this IEnumerable<IEntry> source)
        {
            var parameter = new Parameter();
            foreach (var item in source)
                parameter.Add(item.Name, item.Value);
            return parameter;
        }

        public static bool IsClass(this ITag source,string className)
        {
            if (source.Attributes["class"] == null) return false;
            return source.Attributes["class"].Split(' ').Any(a => a == className);
        }

        public static bool IsId(this ITag source,string id)
        {
            if (source.Attributes["id"] == null) return false;
            return source.Attributes["id"].Split(' ').Any(a => a == id);
        }

        public static HtmlDocument AsHtml(this Response response)
        {
            if (response == null) return null;
            return DataProvider.Html(new DocInfo(response.Client, response.Uri)).TryParse(response.ContentText) as HtmlDocument;
        }

        public static HtmlDocument GetAsHtml(this IRequest request)
        {
            return request.Get().AsHtml();
        }

        public static HtmlDocument PostAsHtml(this IRequest request, object data = null)

        {
            return request.Post(data).AsHtml();
        }
    }
}
