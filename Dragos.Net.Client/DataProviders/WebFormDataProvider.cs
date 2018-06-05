using System;
using System.Linq;

namespace Dragos.Net.Client.DataProviders
{
    public class WebFormDataProvider : IDataProvider
    {
        public string GetValue(object value)
        {
            var result = string.Empty;
            foreach (var prop in value.GetType().GetProperties())
            {
                result += prop.Name + "=" + GetValueString(prop.PropertyType,prop.GetValue(value)) +"&";
            }
            return result.Last() == '&' ? result.Substring(0, result.Length - 1) : result;
        }

        private string GetValueString(Type type,object value)
        {
            if (value == null) return string.Empty;
            if (type == typeof(DateTime)) return ((DateTime)value).ToString("yyyy-MM-dd");
            return System.Net.WebUtility.UrlEncode(value.ToString());

        }

        private string GetName(string name)
        {
            return System.Net.WebUtility.UrlEncode(name);
        }

        public object TryParse(string value)
        {
            return null;
        }
    }
}
