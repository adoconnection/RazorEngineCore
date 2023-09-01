using System;
using System.Threading;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    public interface IRazorEngine
    {
        IRazorEngineCompiledTemplate<T> Compile<T>(string content, Action<IRazorEngineCompilationOptionsBuilder> builderAction = null)
            where T : IRazorEngineTemplate;

        IRazorEngineCompiledTemplate<T> Compile<T>(string content, Action<IRazorEngineCompilationOptionsBuilder> builderAction, CancellationToken cancellationToken)
            where T : IRazorEngineTemplate;

        Task<IRazorEngineCompiledTemplate<T>> CompileAsync<T>(string content, Action<IRazorEngineCompilationOptionsBuilder> builderAction = null)
            where T : IRazorEngineTemplate;

        Task<IRazorEngineCompiledTemplate<T>> CompileAsync<T>(string content, Action<IRazorEngineCompilationOptionsBuilder> builderAction, CancellationToken cancellationToken)
            where T : IRazorEngineTemplate;

        IRazorEngineCompiledTemplate Compile(string content, Action<IRazorEngineCompilationOptionsBuilder> builderAction = null);

        IRazorEngineCompiledTemplate Compile(string content, Action<IRazorEngineCompilationOptionsBuilder> builderAction, CancellationToken cancellationToken);

        Task<IRazorEngineCompiledTemplate> CompileAsync(string content, Action<IRazorEngineCompilationOptionsBuilder> builderAction = null);

        Task<IRazorEngineCompiledTemplate> CompileAsync(string content, Action<IRazorEngineCompilationOptionsBuilder> builderAction, CancellationToken cancellationToken);
    }
}