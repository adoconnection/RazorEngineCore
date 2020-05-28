using System.Threading.Tasks;

namespace RazorEngineCore
{
    public interface IRazorEngineTemplate
    {
        dynamic Model { get; set; }
        void WriteLiteral(string literal = null);
        void Write(object obj = null);
        void BeginWriteAttribute(string name, string prefix, int prefixOffset, string suffix, int suffixOffset, int attributeValuesCount);
        void WriteAttributeValue(string prefix, int prefixOffset, object value, int valueOffset, int valueLength, bool isLiteral);
        void EndWriteAttribute();
        Task ExecuteAsync();
        string Result();
    }
}