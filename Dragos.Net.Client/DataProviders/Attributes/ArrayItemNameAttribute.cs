namespace Dragos.Net.Client.DataProviders.Attributes
{
    public class ArrayItemNameAttribute :System.Attribute
    {
        public string Name { get; }
        public ArrayItemNameAttribute(string name)
        {
            Name = name;
        }
    }
}
