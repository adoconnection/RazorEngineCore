using System;
using System.IO;
using System.Linq;
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
        
        public string Run<T>(T model = null)
            where T : class
        {
            if (model?.IsAnonymous() == true)
            {
                return RunAsync((object)model).GetAwaiter().GetResult();
            }
            
            return RunAsync(model).GetAwaiter().GetResult();
        }
        
        public async Task<string> RunAsync(object model = null)
        {
            if (model != null && model.IsAnonymous() == true)
            {
                model = new AnonymousTypeWrapper(model);
            }

            IRazorEngineTemplateBase instance = (IRazorEngineTemplateBase)Activator.CreateInstance(this.templateType);
            instance.Model = model;
            await instance.ExecuteAsync();
            return instance.Result();
        }
        
        public async Task<string> RunAsync<TModel>(TModel model = null)
            where TModel : class
        {
            if (model?.IsAnonymous() == true)
            {
                return RunAsync((object)model).GetAwaiter().GetResult();
            }
            
            IRazorEngineTemplateBase instance = (IRazorEngineTemplateBase)Activator.CreateInstance(this.templateType);
            
            // Find the correct property to update via reflection.
            // As IRazorEngineTemplateBase<T> inherits from IRazorEngineTemplateBase and the both have `Model`
            var propertyInfo = instance.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty)
                .FirstOrDefault(fd => fd.Name.Equals(nameof(IRazorEngineTemplateBase.Model)) && fd.PropertyType == typeof(TModel));

            if (propertyInfo != null)
            {
                propertyInfo.SetValue(instance, model);
            }
            else
            {
                instance.Model = model;
            }
            
            await instance.ExecuteAsync();
            return instance.Result();
        }
    }
}