using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace Dragos.Net.Client
{
    public class Cookie
    {
        public string Name { get; }

        public string Value { get;  }

        public Cookie(string name,string value)
        {
            this.Name = name;
            this.Value = value;
        }
    }


    public class CookieCollection : IEnumerable<Cookie>
    {
        private List<Cookie> _cookies = new List<Cookie>();

        public void AddOrUpdate(string name,string value)
        {
            if (!this.Has(name))
                _cookies.Add(new Cookie(name.ToLower(), value));
            else Replace(name, value);
        }

        public void Add(CookieCollection collection)
        {
            foreach (var item in collection)
                this.AddOrUpdate(item.Name, item.Value);
        }

    

        public void Clear()
        {
            this._cookies.Clear();
        }

     

        public string Get(string name)
        {
            var firstOrDefault = this._cookies.FirstOrDefault(x => x.Name == name);
            if (firstOrDefault == null) return string.Empty;
            return firstOrDefault.Value;
        }

        public void Replace(string name,string value)
        {
            var item = this._cookies.First(x => x.Name == name.ToLower());
            var index = this._cookies.IndexOf(item);
            this._cookies[index] = new Cookie(name, value);
        }

        public void Remove(string name)
        {
            if (!Has(name)) return;
            var item = this._cookies.First(x => x.Name == name.ToLower());
            this._cookies.Remove(item);
        }

        public bool Has(string name)
        {
            return _cookies.Any(x => x.Name == name.ToLower());
        }

        public IEnumerator<Cookie> GetEnumerator()
        {
            return _cookies.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public string this[string name]
        {
            get
            {
                return this.Get(name);
            }
            set
            {
                this.AddOrUpdate(name, value);
            }
        }
    }
}
