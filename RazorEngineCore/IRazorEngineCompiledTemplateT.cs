using System;
using System.IO;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    public interface IRazorEngineCompiledTemplate<out T, R> where T : IRazorEngineTemplate<R>
    {
        void SaveToStream(Stream stream);
        Task SaveToStreamAsync(Stream stream);
        void SaveToFile(string fileName);
        Task SaveToFileAsync(string fileName);
        R Run(Action<T> initializer);
        Task<R> RunAsync(Action<T> initializer);
    }
}