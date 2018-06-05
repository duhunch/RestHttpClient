using System;

namespace Dragos.Data.Imitation
{
    public interface IDataImitationProvider
    {
        bool Is(Type type);
        object Imitate(DataImitateProvider provider, Type imitateType, object obj);
    }
}
