using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RazorEngineCore
{
    public class RazorEngineCompiledTemplate<T> : RazorEngineCompiledTemplateBase, IRazorEngineCompiledTemplate<T> where T : IRazorEngineTemplate
    {

        internal RazorEngineCompiledTemplate(RazorEngineCompiledTemplateMeta meta)
        {
            this.Meta = meta;

            Assembly assembly = Assembly.Load(meta.AssemblyByteCode, meta.PdbByteCode);
            this.TemplateType = assembly.GetType(meta.TemplateNamespace + ".Template");
        }

        public static RazorEngineCompiledTemplate<T> LoadFromFile(string fileName)
        {
            return LoadFromFileAsync(fileName).GetAwaiter().GetResult();
        }
        
        public static async Task<RazorEngineCompiledTemplate<T>> LoadFromFileAsync(string fileName)
        {
            using (FileStream fileStream = new FileStream(
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

        public static IRazorEngineCompiledTemplate<T> LoadFromStream(Stream stream)
        {
            return LoadFromStreamAsync(stream).GetAwaiter().GetResult();
        }
        
        public static async Task<RazorEngineCompiledTemplate<T>> LoadFromStreamAsync(Stream stream)
        {
            return new RazorEngineCompiledTemplate<T>(await RazorEngineCompiledTemplateMeta.Read(stream));
        }

        public string Run(Action<T> initializer)
        {
            return this.RunAsync(initializer).GetAwaiter().GetResult();
        }
        
        public async Task<string> RunAsync(Action<T> initializer)
        {
            T instance = (T) Activator.CreateInstance(this.TemplateType);
            initializer(instance);

            if (this.IsDebuggerEnabled && instance is RazorEngineTemplateBase instance2)
            {
                instance2.Breakpoint = System.Diagnostics.Debugger.Break;
            }

            await instance.ExecuteAsync();

            return await instance.ResultAsync();
		}


    }
}