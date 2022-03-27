using System;
using System.IO.Pipelines;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    public interface IRazorEngine
    {
        IRazorEngineCompiledTemplate<T> Compile<T>(string content, Action<IRazorEngineCompilationOptionsBuilder> builderAction = null) 
            where T : IRazorEngineTemplate<T>;
        
        Task<IRazorEngineCompiledTemplate<T>> CompileAsync<T>(string content, Action<IRazorEngineCompilationOptionsBuilder> builderAction = null) 
            where T : IRazorEngineTemplate<T>;
        
        IRazorEngineCompiledTemplate<string> Compile(string content, Action<IRazorEngineCompilationOptionsBuilder> builderAction = null);
        
        Task<IRazorEngineCompiledTemplate<string>> CompileAsync(string content, Action<IRazorEngineCompilationOptionsBuilder> builderAction = null);

    }
}