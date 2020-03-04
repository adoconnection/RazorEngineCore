using System;
using System.IO;
using System.Reflection;

namespace RazorEngineCore
{
    public class RazorEngineCompiledTemplate<T> where T : RazorEngineTemplateBase
    {
        private readonly MemoryStream assemblyByteCode;
        private readonly Type templateType;

        internal RazorEngineCompiledTemplate(MemoryStream assemblyByteCode)
        {
            this.assemblyByteCode = assemblyByteCode;

            Assembly assembly = Assembly.Load(assemblyByteCode.ToArray());
            this.templateType = assembly.GetType("TemplateNamespace.Template");
        }

        public static RazorEngineCompiledTemplate<T> LoadFromFile(string fileName)
        {
            return new RazorEngineCompiledTemplate<T>(new MemoryStream(File.ReadAllBytes(fileName)));
        }

        public static RazorEngineCompiledTemplate<T> LoadFromStream(Stream stream)
        {
            MemoryStream memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            memoryStream.Position = 0;

            return new RazorEngineCompiledTemplate<T>(memoryStream);
        }

        public void SaveToStream(Stream stream)
        {
            this.assemblyByteCode.CopyTo(stream);
        }

        public void SaveToFile(string fileName)
        {
            File.WriteAllBytes(fileName, this.assemblyByteCode.ToArray());
        }

        public string Run(Action<T> initializer)
        {
            T instance = (T) Activator.CreateInstance(this.templateType);
            initializer(instance);
            instance.ExecuteAsync().Wait();
            return instance.Result();
        }
    }
}