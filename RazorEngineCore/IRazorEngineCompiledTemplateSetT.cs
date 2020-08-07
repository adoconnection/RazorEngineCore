using System;
using System.IO;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    public interface IRazorEngineCompiledTemplateSet<out T>
        where T : IRazorEngineTemplate
    {
        void SaveToStream(Stream stream);
        
        Task SaveToStreamAsync(Stream stream);
        
        void SaveToFile(string fileName);
        
        Task SaveToFileAsync(string fileName);
        
        string Run(string templateClassName, Action<T> initializer);
        
        Task<string> RunAsync(string templateClassName, Action<T> initializer);
    }
}