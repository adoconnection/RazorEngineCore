using System.IO;
using System;
using System.Text;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    public abstract class RazorEngineCompiledTemplateBase
    {
        protected RazorEngineCompiledTemplateMeta Meta { get; set; }
        protected Type TemplateType { get; set; }

        protected bool IsDebuggerEnabled { get; set; }

        public void SaveToFile(string fileName)
        {
            this.SaveToFileAsync(fileName).GetAwaiter().GetResult();
        }

        public async Task SaveToFileAsync(string fileName)
        {
            using (FileStream fileStream = new FileStream(
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

        public async Task SaveToStreamAsync(Stream stream)
        {
            await this.Meta.Write(stream);
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