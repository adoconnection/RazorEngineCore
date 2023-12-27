using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    public class RazorEngineCompiledTemplate : RazorEngineCompiledTemplateBase, IRazorEngineCompiledTemplate
    {        
        internal RazorEngineCompiledTemplate(RazorEngineCompiledTemplateMeta meta)
        {
            this.Meta = meta;

            Assembly assembly = Assembly.Load(meta.AssemblyByteCode, meta.PdbByteCode);
            this.TemplateType = assembly.GetType(meta.TemplateNamespace + ".Template");
        }

        public static RazorEngineCompiledTemplate LoadFromFile(string fileName)
        {
            return LoadFromFileAsync(fileName).GetAwaiter().GetResult();
        }

        public static async Task<RazorEngineCompiledTemplate> LoadFromFileAsync(string fileName)
        {
#if NETSTANDARD2_0
            using (FileStream fileStream = new FileStream(
#else
            await using (FileStream fileStream = new FileStream(
#endif
                       path: fileName, 
                       mode: FileMode.Open, 
                       access: FileAccess.Read,
                       share: FileShare.None,
                       bufferSize: 4096, 
                       useAsync: true))
            {
                return await LoadFromStreamAsync(fileStream);
            }
        }
        
        public static IRazorEngineCompiledTemplate LoadFromStream(Stream stream)
        {
            return LoadFromStreamAsync(stream).GetAwaiter().GetResult();
        }
        
        public static async Task<RazorEngineCompiledTemplate> LoadFromStreamAsync(Stream stream)
        {
            return new RazorEngineCompiledTemplate(await RazorEngineCompiledTemplateMeta.Read(stream));
        }
        
        public string Run(object model = null)
        {
            return this.RunAsync(model).GetAwaiter().GetResult();
        }

        public async Task<string> RunAsync(object model = null)
        {
            if (model != null && model.IsAnonymous())
            {
                model = new AnonymousTypeWrapper(model);
            }

            IRazorEngineTemplate instance = (IRazorEngineTemplate)Activator.CreateInstance(this.TemplateType);
            instance.Model = model;

            if (this.IsDebuggerEnabled && instance is RazorEngineTemplateBase instance2)
            {
                instance2.Breakpoint = System.Diagnostics.Debugger.Break;
            }

            await instance.ExecuteAsync();

            return await instance.ResultAsync();
        }
    }
}