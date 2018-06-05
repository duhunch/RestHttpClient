using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dragos.Data.Attributes;

namespace Dragos.Data.Imitation
{
    public class DataImitateProvider 
    {
        private readonly List<IDataImitationProvider> _imiateList = new List<IDataImitationProvider>();

        public DataImitateProvider()
        {
            this.Add(new StructImitationProvider());
            this.Add(new DynamicImitationProvider());
            this.Add(new ArrayImitationProvider());
            this.Add(new ObjectImitationProvider());
        }

        public bool Is(Type type)
        {
            return _imiateList.Any(x => x.Is(type));
        }

        public void Add(IDataImitationProvider provider)
        {
            this._imiateList.Add(provider);
        }


        public object Imitate(Type imitateType, object obj)
        {
            foreach (var imitate in _imiateList)
            {
                if (imitate.Is(obj.GetType()))
                   return imitate.Imitate(this, imitateType, obj);
            }
            return null;
        }

        public void SetValue(PropertyInfo prop, object target, object value)
        {
            var v = value;
            var isConvertable = typeof(Dragos.Data.IConvertable).IsAssignableFrom(prop.PropertyType);
            if (isConvertable)
            {
                var targetProp = prop.GetValue(target);
                v = prop.PropertyType.InvokeMember("Convert", BindingFlags.InvokeMethod, null, targetProp, new[] { v });
            }
            prop.SetValue(target, v);
        }

        public string GetName(PropertyInfo info)
        {
            var dataName = info.GetCustomAttribute<DataNameAttribute>();
            if (dataName != null)
                return dataName.Name;
            return info.Name;
        }
    }
}
