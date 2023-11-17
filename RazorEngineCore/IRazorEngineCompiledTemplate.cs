using System.IO;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    public interface IRazorEngineCompiledTemplate
    {
        void SaveToStream(Stream stream);
        Task SaveToStreamAsync(Stream stream);
        void SaveToFile(string fileName);
        Task SaveToFileAsync(string fileName);
        void EnableDebugging(string debuggingOutputDirectory = null);
        string Run(object model = null);
        Task<string> RunAsync(object model = null);
    }
}