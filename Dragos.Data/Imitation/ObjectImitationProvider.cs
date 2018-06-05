using System;
using System.Linq;

namespace Dragos.Data.Imitation
{
    public class ObjectImitationProvider :IDataImitationProvider
    {
        public bool Is(Type type)
        {
            return typeof(Array) == type;
        }

        public object Imitate(DataImitateProvider provider, Type imitateType, object obj)
        {
            if (obj == null) return null;
            var props = obj.GetType().GetProperties();
            var newModel = Activator.CreateInstance(imitateType);
            foreach (var prop in props)
            {
                var name = provider.GetName(prop);
                var modelProp = imitateType.GetProperties().FirstOrDefault(x => x.Name == name);
                if(modelProp == null) // böyle bir property yok
                    continue;
                var value = prop.GetValue(obj);
                provider.SetValue(prop,newModel,provider.Imitate(value.GetType(), value));
            }
            return newModel;
        }
    }
}
