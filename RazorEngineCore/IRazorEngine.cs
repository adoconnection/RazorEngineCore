using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    public interface IRazorEngine
    {
        IRazorEngineCompiledTemplate<T> Compile<T>(string content, Action<IRazorEngineCompilationOptionsBuilder> builderAction = null) 
            where T : IRazorEngineTemplate;
        
        Task<IRazorEngineCompiledTemplate<T>> CompileAsync<T>(string content, Action<IRazorEngineCompilationOptionsBuilder> builderAction = null) 
            where T : IRazorEngineTemplate;
        
        IRazorEngineCompiledTemplate Compile(string content, Action<IRazorEngineCompilationOptionsBuilder> builderAction = null);
        
        Task<IRazorEngineCompiledTemplate> CompileAsync(string content, Action<IRazorEngineCompilationOptionsBuilder> builderAction = null);

        IRazorEngineCompiledTemplateSet<T> CompileSet<T>(Dictionary<string, string> contents,
            Action<IRazorEngineCompilationOptionsBuilder> builderAction = null, List<string> csharpFiles = null)
            where T : IRazorEngineTemplate;

        Task<IRazorEngineCompiledTemplateSet<T>> CompileSetAsync<T>(Dictionary<string, string> contents,
            Action<IRazorEngineCompilationOptionsBuilder> builderAction = null, List<string> csharpFiles = null)
            where T : IRazorEngineTemplate;
        
        IRazorEngineCompiledTemplateSet CompileSet(Dictionary<string, string> contents,
            Action<IRazorEngineCompilationOptionsBuilder> builderAction = null, List<string> csharpFiles = null);

        Task<IRazorEngineCompiledTemplateSet> CompileSetAsync(Dictionary<string, string> contents,
            Action<IRazorEngineCompilationOptionsBuilder> builderAction = null, List<string> csharpFiles = null);
    }
}