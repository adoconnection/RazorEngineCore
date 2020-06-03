using System;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    public interface IRazorEngine
    {
        IRazorEngineCompiledTemplate<T> Compile<T>(string content, Action<RazorEngineCompilationOptionsBuilder> builderAction = null) 
            where T : IRazorEngineTemplate;
        
        Task<IRazorEngineCompiledTemplate<T>> CompileAsync<T>(string content, Action<RazorEngineCompilationOptionsBuilder> builderAction = null) 
            where T : IRazorEngineTemplate;
        
        IRazorEngineCompiledTemplate Compile(string content, Action<RazorEngineCompilationOptionsBuilder> builderAction = null);
        
        Task<IRazorEngineCompiledTemplate> CompileAsync(string content, Action<RazorEngineCompilationOptionsBuilder> builderAction = null);
    }
}