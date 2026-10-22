using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    public class RazorEngineCompiledTemplate : RazorEngineCompiledTemplateBase, IRazorEngineCompiledTemplate
    {        
        internal RazorEngineCompiledTemplate(RazorEngineCompiledTemplateMeta meta, bool enableUnloading = false)
        {
            this.Meta = meta;
            this.LoadTemplateType(enableUnloading);
        }

        public static RazorEngineCompiledTemplate LoadFromFile(string fileName)
        {
            return LoadFromFile(fileName, enableUnloading: false);
        }

        /// <param name="enableUnloading">Load into a collectible AssemblyLoadContext, see <see cref="IRazorEngineCompilationOptionsBuilder.EnableUnloading"/></param>
        public static RazorEngineCompiledTemplate LoadFromFile(string fileName, bool enableUnloading)
        {
            return LoadFromFileAsync(fileName, enableUnloading).GetAwaiter().GetResult();
        }

        public static Task<RazorEngineCompiledTemplate> LoadFromFileAsync(string fileName)
        {
            return LoadFromFileAsync(fileName, enableUnloading: false);
        }

        /// <param name="enableUnloading">Load into a collectible AssemblyLoadContext, see <see cref="IRazorEngineCompilationOptionsBuilder.EnableUnloading"/></param>
        public static async Task<RazorEngineCompiledTemplate> LoadFromFileAsync(string fileName, bool enableUnloading)
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
                return await LoadFromStreamAsync(fileStream, enableUnloading);
            }
        }

        public static IRazorEngineCompiledTemplate LoadFromStream(Stream stream)
        {
            return LoadFromStream(stream, enableUnloading: false);
        }

        /// <param name="enableUnloading">Load into a collectible AssemblyLoadContext, see <see cref="IRazorEngineCompilationOptionsBuilder.EnableUnloading"/></param>
        public static IRazorEngineCompiledTemplate LoadFromStream(Stream stream, bool enableUnloading)
        {
            return LoadFromStreamAsync(stream, enableUnloading).GetAwaiter().GetResult();
        }

        public static Task<RazorEngineCompiledTemplate> LoadFromStreamAsync(Stream stream)
        {
            return LoadFromStreamAsync(stream, enableUnloading: false);
        }

        /// <param name="enableUnloading">Load into a collectible AssemblyLoadContext, see <see cref="IRazorEngineCompilationOptionsBuilder.EnableUnloading"/></param>
        public static async Task<RazorEngineCompiledTemplate> LoadFromStreamAsync(Stream stream, bool enableUnloading)
        {
            return new RazorEngineCompiledTemplate(await RazorEngineCompiledTemplateMeta.Read(stream), enableUnloading);
        }

        public string Run(object model = null)
        {
            return this.RunAsync(model).GetAwaiter().GetResult();
        }

        public async Task<string> RunAsync(object model = null)
        {
            this.ThrowIfDisposed();

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