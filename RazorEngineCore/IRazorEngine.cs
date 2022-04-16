using System;
using System.IO.Pipelines;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    public interface IRazorEngine
    {
        IRazorEngineCompiledTemplate<T, R> Compile<T, R>(string content, Action<IRazorEngineCompilationOptionsBuilder> builderAction = null)
            where T : IRazorEngineTemplate<R>;
        IRazorEngineCompiledTemplate<T, string> Compile<T>(string content, Action<IRazorEngineCompilationOptionsBuilder> builderAction = null) 
            where T : IRazorEngineTemplate<string>;
        
        Task<IRazorEngineCompiledTemplate<T, string>> CompileAsync<T>(string content, Action<IRazorEngineCompilationOptionsBuilder> builderAction = null) 
            where T : IRazorEngineTemplate<string>;
        
        IRazorEngineCompiledTemplate<string> Compile(string content, Action<IRazorEngineCompilationOptionsBuilder> builderAction = null);
        
        Task<IRazorEngineCompiledTemplate<string>> CompileAsync(string content, Action<IRazorEngineCompilationOptionsBuilder> builderAction = null);

    }
}