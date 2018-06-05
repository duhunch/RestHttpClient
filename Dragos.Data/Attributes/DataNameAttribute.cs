namespace Dragos.Data.Attributes
{
    public class DataNameAttribute : System.Attribute
    {
        public string Name { get; }
        public DataNameAttribute(string name)
        {
            Name = name;
        }
    }
}
