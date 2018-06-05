using System;
using System.Xml.XPath;

namespace Dragos.Data.Attributes
{
    public class DataTypeAttribute : System.Attribute
    {
        public TypeCode TypeCode { get; }
        public DataTypeAttribute(TypeCode code)
        {
            TypeCode = code;
        }
    }
}
