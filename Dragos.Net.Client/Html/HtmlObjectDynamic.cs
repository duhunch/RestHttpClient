using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragos.Net.Client.Html
{
    public class HtmlObjectDynamic :System.Dynamic.DynamicObject
    {
        private IDictionary<string, object> _dictionary;

        public HtmlObjectDynamic(params KeyValuePair<string,object>[] parameters)
        {
            this._dictionary = parameters.ToDictionary(x => x.Key, x => x.Value);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = GetProperty(binder.Name);
            return true;
        }

        private object GetProperty(string name)
        {
            if (!_dictionary.ContainsKey(name))
                return _dictionary[name];
            return null;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            result = GetProperty((string)indexes[0]);
            return true;
        }
    }
}
