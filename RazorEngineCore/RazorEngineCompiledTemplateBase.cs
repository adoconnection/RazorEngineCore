using System.IO;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    public abstract class RazorEngineCompiledTemplateBase : IDisposable
    {
        protected RazorEngineCompiledTemplateMeta Meta { get; set; }
        protected Type TemplateType { get; set; }

        protected bool IsDebuggerEnabled { get; set; }

        private bool isDisposed;

#if !NETSTANDARD2_0
        private RazorTemplateLoadContext loadContext;
#endif

        private protected void LoadTemplateType(bool enableUnloading)
        {
#if NETSTANDARD2_0
            Assembly assembly = Assembly.Load(this.Meta.AssemblyByteCode, this.Meta.PdbByteCode);
#else
            Assembly assembly;

            if (enableUnloading)
            {
                this.loadContext = new RazorTemplateLoadContext();

                using MemoryStream assemblyStream = new MemoryStream(this.Meta.AssemblyByteCode);
                using MemoryStream pdbStream = this.Meta.PdbByteCode?.Length > 0
                    ? new MemoryStream(this.Meta.PdbByteCode)
                    : null;

                assembly = this.loadContext.LoadFromStream(assemblyStream, pdbStream);
            }
            else
            {
                assembly = Assembly.Load(this.Meta.AssemblyByteCode, this.Meta.PdbByteCode);
            }
#endif
            this.TemplateType = assembly.GetType(this.Meta.TemplateNamespace + ".Template");
        }

        private protected void ThrowIfDisposed()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }

        public void Dispose()
        {
            this.isDisposed = true;
            this.TemplateType = null;
#if !NETSTANDARD2_0
            // no finalizer needed: a dropped collectible AssemblyLoadContext unloads itself from its own finalizer,
            // calling Unload() from ours races with it and crashes the runtime on .NET 6
            this.loadContext?.Unload();
            this.loadContext = null;
#endif
        }

        public void SaveToFile(string fileName)
        {
            this.SaveToFileAsync(fileName).GetAwaiter().GetResult();
        }

        public async Task SaveToFileAsync(string fileName)
        {
#if NETSTANDARD2_0
            using (FileStream fileStream = new FileStream(
#else
            await using (FileStream fileStream = new FileStream(
#endif
                       path: fileName,
                       mode: FileMode.OpenOrCreate,
                       access: FileAccess.Write,
                       share: FileShare.None,
                       bufferSize: 4096,
                       useAsync: true))
            {
                await this.SaveToStreamAsync(fileStream);
            }
        }

        public void SaveToStream(Stream stream)
        {
            this.SaveToStreamAsync(stream).GetAwaiter().GetResult();
        }

        public Task SaveToStreamAsync(Stream stream)
        {
            return this.Meta.Write(stream);
        }

        public void EnableDebugging(string debuggingOutputDirectory = null)
        {
            if (this.Meta.PdbByteCode == null || this.Meta.PdbByteCode.Length == 0 || string.IsNullOrWhiteSpace(this.Meta.TemplateSource))
            {
                throw new RazorEngineException("No debugging info available, compile template with builder.IncludeDebuggingInfo(); option");
            }

            File.WriteAllText(Path.Combine(debuggingOutputDirectory ?? ".", this.Meta.TemplateFileName), this.Meta.TemplateSource);

            this.IsDebuggerEnabled = true;
        }

    }
}