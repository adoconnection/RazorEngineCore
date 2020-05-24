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
            this.attributeSuffix = suffix;
            this.stringBuilder.Append(prefix);
            this.stringBuilder.Append(this.attributePrefix);
        }

        public void WriteAttributeValue(string prefix, int prefixOffset, object value, int valueOffset, int valueLength, bool isLiteral)
        {
            this.stringBuilder.Append(prefix);
            this.stringBuilder.Append(value);
        }

        public void EndWriteAttribute()
        {
            this.stringBuilder.Append(this.attributeSuffix);
            this.attributeSuffix = null;
        }

        public virtual Task ExecuteAsync()
        {
            return Task.CompletedTask;
        }

        public string Result()
        {
            return this.stringBuilder.ToString();
        }
    }
}