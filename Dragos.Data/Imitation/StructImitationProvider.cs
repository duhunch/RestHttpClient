using System;
using System.Linq;

namespace Dragos.Data.Imitation
{
    public class StructImitationProvider :IDataImitationProvider
    {
        public bool Is(Type type)
        {
            var types = new [] {typeof(string)};
            return type.IsValueType || types.Any(x => x == type);
        }

        public object Imitate(DataImitateProvider provider, Type imitateType, object obj)
        {
            return obj;
        }
    }
}
