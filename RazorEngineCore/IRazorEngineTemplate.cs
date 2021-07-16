using System.Threading.Tasks;

namespace RazorEngineCore
{
    public interface IRazorEngineTemplate
    {
        dynamic Model { get; set; }
        void WriteLiteral(string literal = null);
        
        Task WriteLiteralAsync(string literal = null);
        
        void Write(object obj = null);
        
        Task WriteAsync(object obj = null);
        
        void BeginWriteAttribute(string name, string prefix, int prefixOffset, string suffix, int suffixOffset, int attributeValuesCount);
        
        Task BeginWriteAttributeAsync(string name, string prefix, int prefixOffset, string suffix, int suffixOffset, int attributeValuesCount);
        
        void WriteAttributeValue(string prefix, int prefixOffset, object value, int valueOffset, int valueLength, bool isLiteral);
        
        Task WriteAttributeValueAsync(string prefix, int prefixOffset, object value, int valueOffset, int valueLength, bool isLiteral);
        
        void EndWriteAttribute();
        
        Task EndWriteAttributeAsync();
        
        void Execute();
        
        Task ExecuteAsync();
        
        string Result();
        
        Task<string> ResultAsync();
    }
}