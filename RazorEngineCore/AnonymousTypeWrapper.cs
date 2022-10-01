using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace RazorEngineCore
{
    public class AnonymousTypeWrapper : DynamicObject
    {
        private readonly object model;

        public AnonymousTypeWrapper(object model)
        {
            this.model = model;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            PropertyInfo propertyInfo = this.model.GetType().GetProperty(binder.Name);

            if (propertyInfo == null)
            {
                result = null;
                return false;
            }

            result = propertyInfo.GetValue(this.model, null);

            if (result == null)
            {
                return true;
            }

            var type = result.GetType();

            if (result.IsAnonymous())
            {
                result = new AnonymousTypeWrapper(result);
            }

            if (result is IDictionary dictionary)
            {
                List<object> keys = new List<object>();

                foreach(object key in dictionary.Keys)
                {
                    keys.Add(key);
                }

                foreach(object key in keys)
                {
                    if (dictionary[key].IsAnonymous())
                    {
                        dictionary[key] = new AnonymousTypeWrapper(dictionary[key]);
                    }
                }
            }
            else if (result is IEnumerable enumer && !(result is string))
            {
                result = enumer.Cast<object>()
                        .Select(e =>
                        {
                            if (e.IsAnonymous())
                            {
                                return new AnonymousTypeWrapper(e);
                            }

                            return e;
                        })
                        .ToList();
            }


            return true;
        }
    }
}