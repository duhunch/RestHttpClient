using System.Collections;
using Dragos.Data.Attributes;

namespace Dragos.Net.Client.DataProviders.XmlRpc
{
    public interface IParamValue
    {
        object Value { get; }
    }

    public class IntParam : IParamValue
    {
        [DataName("int")]
        public object Value { get; }

        public IntParam(object value)
        {
            Value = value;
        }
    }

    public class DoubleParam : IParamValue
    {
        [DataName("double")]
        public object Value { get; }

        public DoubleParam(object value)
        {
            Value = value;
        }
    }
    public class BooleanParam : IParamValue
    {
        [DataName("boolean")]
        public object Value { get; }

        public BooleanParam(object value)
        {
            Value = value;
        }
    }

    public class StringParam : IParamValue
    {
        [DataName("string")]
        public object Value { get; }

        public StringParam(object value)
        {
            Value = value;
        }
    }

    public class DateTimeParam :IParamValue
    {
        [DataName("dateTime.iso8601")]
        public object Value { get; }

        public DateTimeParam(object value)
        {
            Value = value;
        }
    }

    public class ArrayParam:IParamValue
    {
        [DataName("array")]
        public object Value { get; }

        public ArrayParam(IEnumerable values)
        {
            Value = values;
        }
    }



    //public class MethodName : IParamValue
    //{
    //    [DataName("methodName")]
    //    public object Value { get; }

    //    public MethodName(string value)
    //    {
    //        this.Value = value;
    //    }
    //}

}
