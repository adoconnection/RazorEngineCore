using System;
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

            if (type.IsArray)
            {
                result = ((IEnumerable<object>)result).Select(e => new AnonymousTypeWrapper(e)).ToList();
            }

            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var methodInfos = this.model.GetType().GetMethods().Where(m => m.Name == binder.Name);

            if (methodInfos == null)
            {
                result = null;
                return false;
            }
            var methodInfo = methodInfos.Where(m => binder.CallInfo.ArgumentCount == m.GetParameters().Count()).FirstOrDefault(m =>
               {
                   int ind = 0;
                   return m.GetParameters().Aggregate(true, (match, next) => match && next.ParameterType.IsAssignableFrom(args[ind++].GetType()));
               });

            if (methodInfo != null)
            {
                result = methodInfo.Invoke(this.model, args);
                return true;
            }

            var newArgs = new List<object>();

            // handle paramater array
            methodInfo = methodInfos.Where(m => m.GetParameters().Last().CustomAttributes.Any(attr => attr.AttributeType == typeof(ParamArrayAttribute)))?
                .FirstOrDefault(m =>
                {
                    var @params = m.GetParameters();
                    int standaloneArgNum = @params.Count() - 1;
                    if (standaloneArgNum > binder.CallInfo.ArgumentCount)
                    {
                        return false;
                    }
                    for (var index = 0; index < standaloneArgNum; ++index)
                    {
                        if (!@params[index].ParameterType.IsAssignableFrom(args[index].GetType()))
                        {
                            return false;
                        }
                        newArgs.Add(args[index]);
                    }

                    var elementType = @params[standaloneArgNum].ParameterType.GetElementType();
                    var paramArray = Array.CreateInstance(elementType, binder.CallInfo.ArgumentCount - standaloneArgNum);
                    for (var index = standaloneArgNum; index < args.Length; ++index)
                    {
                        if (!elementType.IsAssignableFrom(args[index].GetType()))
                        {
                            return false;
                        }
                        paramArray.SetValue(args[index], index-standaloneArgNum);
                    }
                    newArgs.Add(paramArray);
                    return true;
                });

            if (methodInfo == null)
            {
                result = null;
                return false;
            }
            result = methodInfo.Invoke(this.model, newArgs.ToArray());
            return true;
        }
    }
}