using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    public class RazorEngineCompiledTemplateSet : IRazorEngineCompiledTemplateSet
    {
        private readonly MemoryStream assemblyByteCode;
        private readonly Assembly assembly;
        private readonly string templateNamespace;

        internal RazorEngineCompiledTemplateSet(MemoryStream assemblyByteCode, string templateNamespace = "TemplateNamespace")
        {
            this.assemblyByteCode = assemblyByteCode;
            this.templateNamespace = templateNamespace;

            assembly = Assembly.Load(assemblyByteCode.ToArray());
        }

        public static IRazorEngineCompiledTemplateSet LoadFromFile(string fileName, string templateNamespace = "TemplateNamespace")
        {
            return LoadFromFileAsync(fileName: fileName, templateNamespace).GetAwaiter().GetResult();
        }

        public static async Task<IRazorEngineCompiledTemplateSet> LoadFromFileAsync(string fileName, string templateNamespace = "TemplateNamespace")
        {
            MemoryStream memoryStream = new MemoryStream();
            
            using (FileStream fileStream = new FileStream(
                path: fileName, 
                mode: FileMode.Open, 
                access: FileAccess.Read,
                share: FileShare.None,
                bufferSize: 4096, 
                useAsync: true))
            {
                await fileStream.CopyToAsync(memoryStream);
            }
            
            return new RazorEngineCompiledTemplateSet(memoryStream, templateNamespace);
        }
        
        public static IRazorEngineCompiledTemplateSet LoadFromStream(Stream stream, string templateNamespace = "TemplateNamespace")
        {
            return LoadFromStreamAsync(stream, templateNamespace).GetAwaiter().GetResult();
        }
        
        public static async Task<IRazorEngineCompiledTemplateSet> LoadFromStreamAsync(Stream stream, string templateNamespace = "TemplateNamespace")
        {
            MemoryStream memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            
            return new RazorEngineCompiledTemplateSet(memoryStream, templateNamespace);
        }

        public void SaveToStream(Stream stream)
        {
            this.SaveToStreamAsync(stream).GetAwaiter().GetResult();
        }

        public Task SaveToStreamAsync(Stream stream)
        {
            return this.assemblyByteCode.CopyToAsync(stream);
        }

        public void SaveToFile(string fileName)
        {
            this.SaveToFileAsync(fileName).GetAwaiter().GetResult();
        }
        
        public Task SaveToFileAsync(string fileName)
        {
            using (FileStream fileStream = new FileStream(
                path: fileName, 
                mode: FileMode.OpenOrCreate, 
                access: FileAccess.Write,
                share: FileShare.None,
                bufferSize: 4096, 
                useAsync: true))
            {
                return assemblyByteCode.CopyToAsync(fileStream);
            }
        }
        
        public string Run(string templateClassName, object model = null)
        {
            return this.RunAsync(templateClassName, model).GetAwaiter().GetResult();
        }
        
        public async Task<string> RunAsync(string templateClassName, object model = null)
        {
            if (model != null && model.IsAnonymous())
            {
                model = new AnonymousTypeWrapper(model);
            }

            var foo = assembly.GetType(templateNamespace + "." + templateClassName);
            
            IRazorEngineTemplate instance =
                (IRazorEngineTemplate) Activator.CreateInstance(
                    assembly.GetType(templateNamespace + "." + templateClassName));
            instance.Model = model;

            await instance.ExecuteAsync();

            return instance.Result();
        }
    }
}