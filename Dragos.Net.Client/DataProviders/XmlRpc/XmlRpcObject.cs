
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;


namespace Dragos.Net.Client.DataProviders.XmlRpc
{
    public class XmlRpcObject : System.Dynamic.DynamicObject,IEnumerable<KeyValuePair<string,object>>
    {
        private IDictionary<string, object> _dictionary;

        public XmlRpcObject(params KeyValuePair<string, object>[] parameters)
        {
            this._dictionary = parameters.ToDictionary(x => x.Key, x => x.Value);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = GetProperty(binder.Name);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            this[binder.Name] = value;
            return true;
        }


        private object GetProperty(string name)
        {
            if (_dictionary.ContainsKey(name))
                return _dictionary[name];
            return null;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            result = GetProperty((string)indexes[0]);
            return true;
        }

        public object this[string name]
        {
            get { return GetProperty(name); }
            set
            {
                if (_dictionary.ContainsKey(name))
                    _dictionary[name] = value;
                else _dictionary.Add(name, value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }
    }
}
