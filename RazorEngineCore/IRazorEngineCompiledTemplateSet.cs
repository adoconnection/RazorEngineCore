using System.IO;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    public interface IRazorEngineCompiledTemplateSet
    {
        void SaveToStream(Stream stream);
        
        Task SaveToStreamAsync(Stream stream);
        
        void SaveToFile(string fileName);
        
        Task SaveToFileAsync(string fileName);
        
        string Run(string templateClassName, object model = null);
        
        Task<string> RunAsync(string templateClassName, object model = null);
    }
}