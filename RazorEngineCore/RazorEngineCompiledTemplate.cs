using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    public class RazorEngineCompiledTemplate
    {
        private readonly MemoryStream assemblyByteCode;
        protected readonly Type templateType;

        internal RazorEngineCompiledTemplate(MemoryStream assemblyByteCode)
        {
            this.assemblyByteCode = assemblyByteCode;

            Assembly assembly = Assembly.Load(assemblyByteCode.ToArray());
            this.templateType = assembly.GetType("TemplateNamespace.Template");
        }

        public static RazorEngineCompiledTemplate LoadFromFile(string fileName)
        {
            return LoadFromFileAsync(fileName: fileName).GetAwaiter().GetResult();
        }

        public static async Task<RazorEngineCompiledTemplate> LoadFromFileAsync(string fileName)
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
            
            return new RazorEngineCompiledTemplate(assemblyByteCode: memoryStream);
        }
        
        public static RazorEngineCompiledTemplate LoadFromStream(Stream stream)
        {
            return LoadFromStreamAsync(stream).GetAwaiter().GetResult();
        }
        
        public static async Task<RazorEngineCompiledTemplate> LoadFromStreamAsync(Stream stream)
        {
            MemoryStream memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            
            return new RazorEngineCompiledTemplate(memoryStream);
        }
        
        /*public static async Task<T> LoadFromStreamAsync<T>(Stream stream)
            where T: RazorEngineCompiledTemplate, new()
        {
            MemoryStream memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            T instance = (T) Activator.CreateInstance<T>();
                
            return new T(memoryStream);
        }*/

        public void SaveToStream(Stream stream)
        {
            SaveToStreamAsync(stream).GetAwaiter().GetResult();
        }

        public Task SaveToStreamAsync(Stream stream)
        {
            return this.assemblyByteCode.CopyToAsync(stream);
        }

        public void SaveToFile(string fileName)
        {
            SaveToFileAsync(fileName).GetAwaiter().GetResult();
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
        
        public string Run(object model = null)
        {
            return RunAsync(model).GetAwaiter().GetResult();
        }
        
        public async Task<string> RunAsync(object model = null)
        {
            if (model != null && model.IsAnonymous())
            {
                model = new AnonymousTypeWrapper(model);
            }

            RazorEngineTemplateBase instance = (RazorEngineTemplateBase)Activator.CreateInstance(this.templateType);
            instance.Model = model;
            await instance.ExecuteAsync();
            return instance.Result();
        }
    }
}