using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

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

        internal static async Task ReadExactlyAsync(this Stream stream, byte[] buffer)
        {
            int offset = 0;
            while (offset < buffer.Length)
            {
#if NETSTANDARD2_0
                int bytesRead = await stream.ReadAsync(buffer, offset, buffer.Length - offset);
#else
                int bytesRead = await stream.ReadAsync(buffer.AsMemory(offset, buffer.Length - offset));
#endif
                if (bytesRead == 0)
                {
                    throw new EndOfStreamException();
                }

                offset += bytesRead;
            }
        }

        public static async Task<long> ReadLong(this Stream stream)
        {
            byte[] buffer = new byte[8];
            await stream.ReadExactlyAsync(buffer);
            return BitConverter.ToInt64(buffer, 0);
        }

        public static async Task WriteLong(this Stream stream, long value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
#if NETSTANDARD2_0
            await stream.WriteAsync(buffer, 0, buffer.Length);
#else
            await stream.WriteAsync(buffer);
#endif
        }
    }
}