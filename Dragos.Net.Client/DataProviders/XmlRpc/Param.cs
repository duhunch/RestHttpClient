using Dragos.Data.Attributes;

namespace Dragos.Net.Client.DataProviders.XmlRpc
{
    [DataName("param")]
    public class Param
    {
        [DataName("value")]
        public object Value { get; }

        public Param(object value)
        {
            this.Value = value;
        }
    }

    //[DataName("param")]
    //public class ParamWithName
    //{
    //    [DataName("member")]
    //    public Member Member { get; }

    //    public ParamWithName(string name,object value)
    //    {
    //       Member = new Member(name,value);
    //    }
    //}
    //[DataName("member")]
    //public class Member
    //{

    //    [DataName("value")]
    //    public object Value { get; }

    //    [DataName("name")]
    //    public string Name { get; }
    //    public Member(string name, object value)
    //    {
    //        Name = name;
    //        Value = ValueParamFactory.Create(value);
    //    }

    //}
}
