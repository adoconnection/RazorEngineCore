using System.IO;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    public interface IRazorEngineCompiledTemplate<R>
    {
        void SaveToStream(Stream stream);
        Task SaveToStreamAsync(Stream stream);
        void SaveToFile(string fileName);
        Task SaveToFileAsync(string fileName);
        R Run(object model = null);
        Task<R> RunAsync(object model = null);
    }
}