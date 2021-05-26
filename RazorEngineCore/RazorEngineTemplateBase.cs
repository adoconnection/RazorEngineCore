using System.Text;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    public abstract class RazorEngineTemplateBase : IRazorEngineTemplate
    {
        private readonly StringBuilder stringBuilder = new StringBuilder();

        private string attributeSuffix = null;

        public dynamic Model { get; set; }

        public void WriteLiteral(string literal = null)
        {
            WriteLiteralAsync(literal).GetAwaiter().GetResult();
        }

        public virtual Task WriteLiteralAsync(string literal = null)
        {
            this.stringBuilder.Append(literal);
            return Task.CompletedTask;
        }

        public void Write(object obj = null)
        {
            WriteAsync(obj).GetAwaiter().GetResult();
        }

        public virtual Task WriteAsync(object obj = null)
        {
            this.stringBuilder.Append(obj);
            return Task.CompletedTask;
        }

        public void BeginWriteAttribute(string name, string prefix, int prefixOffset, string suffix, int suffixOffset,
            int attributeValuesCount)
        {
            BeginWriteAttributeAsync(name, prefix, prefixOffset, suffix, suffixOffset, attributeValuesCount).GetAwaiter().GetResult();
        }

        public virtual Task BeginWriteAttributeAsync(string name, string prefix, int prefixOffset, string suffix, int suffixOffset, int attributeValuesCount)
        {
            this.attributeSuffix = suffix;
            this.stringBuilder.Append(prefix);
            return Task.CompletedTask;
        }

        public void WriteAttributeValue(string prefix, int prefixOffset, object value, int valueOffset, int valueLength,
            bool isLiteral)
        {
            WriteAttributeValueAsync(prefix, prefixOffset, value, valueOffset, valueLength, isLiteral).GetAwaiter().GetResult();
        }

        public virtual Task WriteAttributeValueAsync(string prefix, int prefixOffset, object value, int valueOffset, int valueLength, bool isLiteral)
        {
            this.stringBuilder.Append(prefix);
            this.stringBuilder.Append(value);
            return Task.CompletedTask;
        }

        public void EndWriteAttribute()
        {
            EndWriteAttributeAsync().GetAwaiter().GetResult();
        }

        public virtual Task EndWriteAttributeAsync()
        {
            this.stringBuilder.Append(this.attributeSuffix);
            this.attributeSuffix = null;
            return Task.CompletedTask;
        }

        public void Execute()
        {
            ExecuteAsync().GetAwaiter().GetResult();
        }

        public virtual Task ExecuteAsync()
        {
            return Task.CompletedTask;
        }

        public virtual string Result()
        {
            return ResultAsync().GetAwaiter().GetResult();
        }

        public virtual Task<string> ResultAsync()
        {
            return Task.FromResult<string>(this.stringBuilder.ToString());
        }
    }
}