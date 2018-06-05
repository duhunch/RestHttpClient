using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;


namespace Dragos.Net.Client
{
    public class ContentProvider
    {

        public string Name { get; }
        public IDataProvider Provider { get; }
        public ContentProvider(string name, IDataProvider provider)
        {
            Name = name;
            Provider = provider;
        }

    }
    public class ContentProviderCollection :IEnumerable<ContentProvider>
    {
        private readonly IDictionary<string,IDataProvider> _providers = new ConcurrentDictionary<string, IDataProvider>();

        public void Add(string contentType, IDataProvider provider)
        {
            if (!_providers.ContainsKey(contentType.ToLower()))
                _providers.Add(new KeyValuePair<string, IDataProvider>(contentType.ToLower(), provider));
        }

        public bool Has(string contentType)
        {
            if (contentType == null) return false;
            return contentType.Split(';').Any(x => _providers.ContainsKey(x));
        }

        public IDataProvider Get(string contentType)
        {
            if (!Has(contentType)) return null;
            foreach (var key in _providers.Keys)
            {
                if (contentType.Split(';').Contains(key))
                    return _providers[key];
            }
            return null;
        }

        public IDataProvider this[string name]
        {
            get { return this.Get(name); }
            set { _providers[name] = value; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<ContentProvider> GetEnumerator()
        {
            return _providers.ToList().Select(x => new ContentProvider(x.Key, x.Value)).GetEnumerator();
        }
    }
}
