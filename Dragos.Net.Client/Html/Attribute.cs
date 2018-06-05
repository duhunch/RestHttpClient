using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Dragos.Net.Client.Html
{
    public class Attribute
    {
        public string Key { get; }
        public string Value { get; }
        public Attribute(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }

    public interface IAttributes :IEnumerable<Attribute>
    {
        bool Exists(string key);
        string this[string key] { get; }
    }

    public class Attributes : IAttributes
    {
        private readonly List<Attribute> _attributes = new List<Attribute>();

        public int Count { get; private set; } = 0;

        public bool IsEmpty
        {
            get { return Count == 0; }
        }

        public string this[string key] => Get(key);
        

        public Attributes(IEnumerable<Attribute> attributes)
        {
          this.Add(attributes);
        }

        public Attributes()
        {
                
        }

        public void Add(string key, string value)
        {
            this._attributes.Add(new Attribute(key, value));
            
        }

        public void Add(Attribute attribute)
        {
            if(attribute == null) return;
            this._attributes.Add(attribute);
            Count++;
        }

        public void Add(IEnumerable<Attribute> attributes)
        {
            foreach (var attribute in attributes)
                this.Add(attribute);
        }

        public string Get(string key)
        {
            var firstOrDefault = this.FirstOrDefault(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
            if (firstOrDefault == null)
                return string.Empty;
            return firstOrDefault.Value;
        }

        public bool Exists(string key)
        {
            return _attributes.Exists(x => x.Key == key);
        }

        public void Remove(string key)
        {
            var item = this._attributes.FirstOrDefault(x => x.Key == key);
            if(item == null)
              return;
            this._attributes.Remove(item);
        }

        public IEnumerator<Attribute> GetEnumerator()
        {
            return _attributes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
