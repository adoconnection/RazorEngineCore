using System.Collections.Generic;
using System.Dynamic;

namespace RazorEngineCore
{
    public static class ObjectExtenders
    {
        public static ExpandoObject ToExpando(this object anonymousObject)
        {
            ExpandoObject expando = new ExpandoObject();
            IDictionary<string, object> dictionary = expando;

            foreach (var property in anonymousObject.GetType().GetProperties())
            {
                dictionary.Add(property.Name, property.GetValue(anonymousObject));
            }

            return expando;
        }
    }
}