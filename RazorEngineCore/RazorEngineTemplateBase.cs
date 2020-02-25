using System.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    public abstract class RazorEngineTemplateBase
    {
        private readonly StringBuilder stringBuilder = new StringBuilder();

        private string attributePrefix = null;
        private string attributeSuffix = null;

        public dynamic Model { get; set; }

        public void WriteLiteral(string literal = null)
        {
            this.stringBuilder.Append(literal);
        }

        public void Write(object obj = null)
        {
            this.stringBuilder.Append(obj);
        }

        public void BeginWriteAttribute(string name, string prefix, int prefixOffset, string suffix, int suffixOffset, int attributeValuesCount)
        {
            this.attributePrefix = prefix;
            this.attributeSuffix = suffix;
        }

        public void WriteAttributeValue(string prefix, int prefixOffset, object value, int valueOffset, int valueLength, bool isLiteral)
        {
            this.stringBuilder.Append(this.attributePrefix);
            this.stringBuilder.Append(value);
            this.stringBuilder.Append(this.attributeSuffix);
        }

        public void EndWriteAttribute()
        {
            this.attributePrefix = null;
            this.attributeSuffix = null;
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