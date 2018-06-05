using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dragos.Data.Imitation
{
   public class ArrayImitationProvider :IDataImitationProvider
    {
        public bool Is(Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type);
        }

        public object Imitate(DataImitateProvider provider, Type imitateType, object arrayObject)
        {
            var listType = typeof(List<>).MakeGenericType(GetElementType(imitateType));
            var list = Activator.CreateInstance(listType);
            foreach (var item in (IEnumerable) arrayObject)
            {
                if(item == null)continue;
                listType.InvokeMember("Add", BindingFlags.InvokeMethod, null, list,
                    new[] {provider.Imitate(GetElementType(imitateType), item)});
            }
            return Return(list,imitateType);
        }

        private Type GetElementType(Type imitateType)
        {

            if (imitateType.IsArray)
                return imitateType.GetElementType();
            if (imitateType.IsGenericType)
                return imitateType.GenericTypeArguments[0];
            return null;
        }

        private static object Return(object returnObject, Type imitateType)
        {
            if (imitateType.IsArray)
                return returnObject.GetType().InvokeMember("ToArray",BindingFlags.InvokeMethod,null,returnObject,new object[0],null);
            if (imitateType == typeof(List<>))
                return returnObject;
            return returnObject;
        }
    }
}
