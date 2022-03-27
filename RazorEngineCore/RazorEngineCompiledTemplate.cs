using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    public class RazorEngineCompiledTemplate<R> : IRazorEngineCompiledTemplate<R>
    {        
        private readonly MemoryStream assemblyByteCode;
        private readonly Type templateType;

        internal RazorEngineCompiledTemplate(MemoryStream assemblyByteCode, string templateNamespace)
        {
            this.assemblyByteCode = assemblyByteCode;

            Assembly assembly = Assembly.Load(assemblyByteCode.ToArray());
            this.templateType = assembly.GetType(templateNamespace + ".Template");
        }

        public static IRazorEngineCompiledTemplate<R> LoadFromFile(string fileName, string templateNamespace = "TemplateNamespace")
        {
            return LoadFromFileAsync(fileName, templateNamespace).GetAwaiter().GetResult();
        }

        public static async Task<IRazorEngineCompiledTemplate<R>> LoadFromFileAsync(string fileName, string templateNamespace = "TemplateNamespace")
        {
            MemoryStream memoryStream = new();
            
            using (FileStream fileStream = new(
                path: fileName, 
                mode: FileMode.Open, 
                access: FileAccess.Read,
                share: FileShare.None,
                bufferSize: 4096, 
                useAsync: true))
            {
                await fileStream.CopyToAsync(memoryStream);
            }
            
            return new RazorEngineCompiledTemplate<R>(memoryStream, templateNamespace);
        }
        
        public static IRazorEngineCompiledTemplate<R> LoadFromStream(Stream stream)
        {
            return LoadFromStreamAsync(stream).GetAwaiter().GetResult();
        }
        
        public static async Task<IRazorEngineCompiledTemplate<R>> LoadFromStreamAsync(Stream stream, string templateNamespace = "TemplateNamespace")
        {
            MemoryStream memoryStream = new();
            await stream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            
            return new RazorEngineCompiledTemplate<R>(memoryStream, templateNamespace);
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
            using FileStream fileStream = new(
                path: fileName,
                mode: FileMode.OpenOrCreate,
                access: FileAccess.Write,
                share: FileShare.None,
                bufferSize: 4096,
                useAsync: true);
            return assemblyByteCode.CopyToAsync(fileStream);
        }
        
        public R Run(object model = null)
        {
            return this.RunAsync(model).GetAwaiter().GetResult();
        }
        
        public async Task<R> RunAsync(object model = null)
        {
            if (model != null && model.IsAnonymous())
            {
                model = new AnonymousTypeWrapper(model);
            }

            IRazorEngineTemplate<R> instance = (IRazorEngineTemplate<R>) Activator.CreateInstance(this.templateType);
            instance.Model = model;

            await instance.ExecuteAsync();

            return await instance.ResultAsync();
        }
    }
}