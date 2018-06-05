using System;
namespace Dragos.Net.Client.DataProviders
{
    public class TextDataProvider :IDataProvider
    {
        public string GetValue(object value)
        {
            return value?.ToString();
        }

        public object TryParse(string value)
        {
            return value;
        }
    }
}
