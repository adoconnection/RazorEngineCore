using System.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    public abstract class RazorEngineTemplateBase
    {
        private readonly StringBuilder stringBuilder = new StringBuilder();
        private string attributeTemplate = "";
        public dynamic Model { get; set; }

        public void WriteLiteral(string literal)
        {
            this.stringBuilder.Append(literal);
        }

        public void Write(object obj)
        {
            this.stringBuilder.Append(obj);
        }

        public void BeginWriteAttribute(string name, string prefix, int prefixOffset, string suffix, int suffixOffset, int attributeValuesCount)
        {
            attributeTemplate = prefix + "{0}" + suffix;
        }

        public void WriteAttributeValue(string prefix, int prefixOffset, object value, int valueOffset, int valueLength, bool isLiteral)
        {
            this.stringBuilder.Append(string.Format(attributeTemplate, value));
        }

        public void EndWriteAttribute()
        {
            attributeTemplate = "";
        }

        public async virtual Task ExecuteAsync()
        {
        }

        public string Result()
        {
            return this.stringBuilder.ToString();
        }
    }
}