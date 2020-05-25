using System.IO;

namespace RazorEngineCore
{
    public class RazorEngineCompiledTemplate<T> : RazorEngineCompiledTemplate
        where T : class
    {
        internal RazorEngineCompiledTemplate(MemoryStream assemblyByteCode) 
            : base(assemblyByteCode)
        {
        }
    }
}