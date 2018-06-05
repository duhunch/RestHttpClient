using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Dragos.Net.Client
{
    //public class ParameterItem
    //{
    //    public string Name { get; }
    //    public object Value { get; }
    //    public ParameterItem(string name, object value)
    //    {
    //        Name = name;
    //        Value = value;
    //    }


    //}
    public class Parameter
    {
        private readonly IDictionary<string, object> _properties = new Dictionary<string, object>();

        public Parameter Add(string name,object value)
        {
            if (value == null) return this;
            if (_properties.ContainsKey(name))
                _properties[name] = value;
            else _properties.Add(name, value);
            return this;
        }

        public Parameter Set(string name,object value)
        {
            if (!this._properties.ContainsKey(name))
                throw new System.IndexOutOfRangeException(name + " not found");
            this._properties[name] = value;
            return this;
        }

        public string ToQueryString()
        {
            var result = string.Empty;
            foreach (var s in _properties)
                result += s.Key + "=" + s.Value + "&";
            
            return result.Last() == '&' ? result.Substring(0, result.Length - 1) : result;
        }

        public static Parameter New()
        {
            return new Parameter();
        }

        public Parameter Clone()
        {
            var parameters = new Parameter();
            foreach (var dic in _properties)
            {
                parameters.Add(dic.Key, dic.Value);
            }
            return parameters;
        }

   
        public object ToObject()
        {
            var arr = new List<KeyValuePair<string, System.Type>>();
            foreach (var prop in _properties)
                arr.Add(new KeyValuePair<string, System.Type>(prop.Key, prop.Value.GetType()));
            var newObject = RuntimeTypeBuilder.CreateNewObject(arr.ToArray());
            foreach(var prop in newObject.GetType().GetProperties())
            {
                var value = _properties[prop.Name];
                prop.SetValue(newObject, value);
            }
            return newObject;
        }

        public IDictionary<string, object> All()
        {
            return _properties.ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
