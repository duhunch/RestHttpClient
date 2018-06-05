namespace Dragos.Net.Client
{
    public class UrlSearch
    {
        public string Name { get; }
        public string Value { get; }
        public UrlSearch(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
    public class Url
    {
        public string Address { get; }

        public string Host { get; }

        public UrlSearch[] Search { get; }

        public Url(string address)
        {
            this.Address = address;
        }
    }
}
