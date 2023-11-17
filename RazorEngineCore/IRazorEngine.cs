using System;
using System.Threading;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    public interface IRazorEngine
    {
        IRazorEngineCompiledTemplate<T> Compile<T>(string content, Action<IRazorEngineCompilationOptionsBuilder> builderAction = null, CancellationToken cancellationToken = default)
            where T : IRazorEngineTemplate;

        Task<IRazorEngineCompiledTemplate<T>> CompileAsync<T>(string content, Action<IRazorEngineCompilationOptionsBuilder> builderAction = null, CancellationToken cancellationToken = default)
            where T : IRazorEngineTemplate;

        IRazorEngineCompiledTemplate Compile(string content, Action<IRazorEngineCompilationOptionsBuilder> builderAction = null, CancellationToken cancellationToken = default);

        Task<IRazorEngineCompiledTemplate> CompileAsync(string content, Action<IRazorEngineCompilationOptionsBuilder> builderAction = null, CancellationToken cancellationToken = default);
    }
}