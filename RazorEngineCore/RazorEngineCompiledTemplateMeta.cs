using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    public class RazorEngineCompiledTemplateMeta
    {
        public byte[] AssemblyByteCode { get; set; }
        public byte[] PdbByteCode { get; set; }
        public string GeneratedSourceCode { get; set; }
        public string TemplateNamespace { get; set; } = "TemplateNamespace";
        public string TemplateSource { get; set; }
        public string TemplateFileName { get; set; }

        public async Task Write(Stream stream)
        {
            await stream.WriteLong(10001);

            await this.WriteBuffer(stream, this.AssemblyByteCode);
            await this.WriteBuffer(stream, this.PdbByteCode);
            await this.WriteString(stream, this.GeneratedSourceCode);
            await this.WriteString(stream, this.TemplateSource);
            await this.WriteString(stream, this.TemplateNamespace);
            await this.WriteString(stream, this.TemplateFileName);
        }

        public static async Task<RazorEngineCompiledTemplateMeta> Read(Stream stream)
        {
            long version = await stream.ReadLong();

            if (version == 10001)
            {
                return await LoadVersion1(stream);
            }

            throw new RazorEngineException("Unable to load template: wrong version");
        }

        private static async Task<RazorEngineCompiledTemplateMeta> LoadVersion1(Stream stream)
        {
            return new RazorEngineCompiledTemplateMeta()
            {
                AssemblyByteCode = await ReadBuffer(stream),
                PdbByteCode = await ReadBuffer(stream),
                GeneratedSourceCode = await ReadString(stream),
                TemplateSource = await ReadString(stream),
                TemplateNamespace = await ReadString(stream),
                TemplateFileName = await ReadString(stream),
            };
        }

        private Task WriteString(Stream stream, string value)
        {
            byte[] buffer = value == null ? null : Encoding.UTF8.GetBytes(value);
            return this.WriteBuffer(stream, buffer);
        }

        private async Task WriteBuffer(Stream stream, byte[] buffer)
        {
            if (buffer == null)
            {
                await stream.WriteLong(0);
                return;
            }

            await stream.WriteLong(buffer.Length);
#if NETSTANDARD2_0
            await stream.WriteAsync(buffer, 0, buffer.Length);
#else
            await stream.WriteAsync(buffer);
#endif
        }

        private static async Task<string> ReadString(Stream stream)
        {
            byte[] buffer = await ReadBuffer(stream);
            return buffer == null ? null : Encoding.UTF8.GetString(buffer);
        }

        private static async Task<byte[]> ReadBuffer(Stream stream)
        {
            long length = await stream.ReadLong();

            if (length == 0)
            {
                return null;
            }

            byte[] buffer = new byte[length];
#if NETSTANDARD2_0
            _ = await stream.ReadAsync(buffer, 0, buffer.Length);
#else
            _ = await stream.ReadAsync(buffer);
#endif
            return buffer;
        }
    }
}
