using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace RazorEngineCore
{
    public static class ObjectExtenders
    {
        public static ExpandoObject ToExpando(this object obj)
        {
            ExpandoObject expando = new ExpandoObject();
            IDictionary<string, object> dictionary = expando;

            foreach (PropertyInfo property in obj.GetType().GetProperties())
            {
                dictionary.Add(property.Name, property.GetValue(obj));
            }

            return expando;
        }

        public static bool IsAnonymous(this object obj)
        {
            Type type = obj.GetType();

            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                   && type.IsGenericType && type.Name.Contains("AnonymousType")
                      && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                   && type.Attributes.HasFlag(TypeAttributes.NotPublic);
        }

        public static long ReadLong(this Stream stream)
        {
            byte[] buffer = new byte[8];
            stream.Read(buffer, 0, 8);

            return BitConverter.ToInt64(buffer, 0);
        }

        public static void WriteLong(this Stream stream, long value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            stream.Write(buffer, 0, buffer.Length);
        }
    }
}