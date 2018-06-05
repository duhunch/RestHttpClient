using Dragos.Data.Attributes;

namespace Dragos.Net.Client.DataProviders.XmlRpc
{
    public enum MethodParamsType
    {
        Array = 1,
        Struct = 2
    }

    [DataName("methodCall")]
    public class MethodCall 
    {
        [DataName("methodName")]
        public string MethodName { get;  }

        [DataName("params")]
        public object[] Params { get; }

        public MethodCall(string methodName, params object[] parameters)
        {
            this.MethodName = methodName;
            Params = parameters;
        }

    }
}
