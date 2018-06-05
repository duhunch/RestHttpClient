using System;
using System.Collections.Generic;
using System.Linq;



namespace Dragos.Data.Imitation
{
    public class DynamicImitationProvider:IDataImitationProvider
    {
        public bool Is(Type type)
        {
            return typeof(IEnumerable<KeyValuePair<string, object>>).IsAssignableFrom(type);
        }

        public object Imitate(DataImitateProvider provider, Type imitateType, object obj)
        {
 
            var newObject = Activator.CreateInstance(imitateType);
            foreach (var item in (IEnumerable<KeyValuePair<string,object>>) obj )
            {
                if(item.Value == null) continue;
                var modelProp = imitateType.GetProperties().FirstOrDefault(x => provider.GetName(x) == item.Key);
                if(modelProp == null) continue;
                var imitateValue = provider.Imitate(item.GetType(), item.Value);
                provider.SetValue(modelProp, newObject, imitateValue);
            }
            return newObject;
        }

    }
}
