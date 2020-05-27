using System;
using System.IO;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    public class RazorEngineCompiledTemplate<T> : RazorEngineCompiledTemplate
        where T : class, IRazorEngineTemplateBase
    {
        internal RazorEngineCompiledTemplate(MemoryStream assemblyByteCode) 
            : base(assemblyByteCode)
        {
        }
        
        public string Run(Action<T> initializer)
        {
            return RunAsync(initializer).GetAwaiter().GetResult();
        }
        
        public async Task<string> RunAsync(Action<T> initializer)
        {
            T instance = (T) Activator.CreateInstance(this.templateType);
            initializer(instance);
            await instance.ExecuteAsync();
            return instance.Result();
        }
    }
}