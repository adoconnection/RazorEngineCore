using System;
using System.IO;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    public class RazorEngineCompiledTemplate<TTemplate> : RazorEngineCompiledTemplate
        where TTemplate : class, IRazorEngineTemplate
    {
        internal RazorEngineCompiledTemplate(MemoryStream assemblyByteCode) 
            : base(assemblyByteCode)
        {
        }
        
        public string Run(Action<TTemplate> initializer)
        {
            return RunAsync(initializer).GetAwaiter().GetResult();
        }

        public async Task<string> RunAsync(Action<TTemplate> initializer)
        {
            TTemplate instance = (TTemplate) Activator.CreateInstance(this.TemplateType);
            initializer(instance);
            await instance.ExecuteAsync();
            return instance.Result();
        }
    }
}