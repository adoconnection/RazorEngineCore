using System;
using System.IO;
using System.Reflection;

namespace RazorEngineCore
{
    public class RazorEngineCompiledTemplate
    {
        private readonly MemoryStream assemblyByteCode;
        private readonly Type templateType;

        internal RazorEngineCompiledTemplate(MemoryStream assemblyByteCode)
        {
            this.assemblyByteCode = assemblyByteCode;

            Assembly assembly = Assembly.Load(assemblyByteCode.ToArray());
            this.templateType = assembly.GetType("TemplateNamespace.Template");
        }

        public static RazorEngineCompiledTemplate LoadFromFile(string fileName)
        {
            return new RazorEngineCompiledTemplate(new MemoryStream(File.ReadAllBytes(fileName)));
        }

        public static RazorEngineCompiledTemplate LoadFromStream(Stream stream)
        {
            MemoryStream memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            memoryStream.Position = 0;

            return new RazorEngineCompiledTemplate(memoryStream);
        }

        public void SaveToStream(Stream stream)
        {
            this.assemblyByteCode.CopyTo(stream);
        }

        public void SaveToFile(string fileName)
        {
            File.WriteAllBytes(fileName, this.assemblyByteCode.ToArray());
        }

        public string Run(object model = null)
        {
            if (model != null && model.IsAnonymous())
            {
                model = new AnonymousTypeWrapper(model);
            }

            RazorEngineTemplateBase instance = (RazorEngineTemplateBase)Activator.CreateInstance(this.templateType);
            instance.Model = model;
            instance.ExecuteAsync().Wait();
            return instance.Result();
        }
    }
}