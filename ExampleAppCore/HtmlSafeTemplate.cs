using RazorEngineCore;

namespace ExampleApp
{
    public class HtmlSafeTemplate : RazorEngineTemplateBase
    {
        class RawContent
        {
            public object Value { get; set; }

            public RawContent(object value)
            {
                Value = value;
            }
        }

        public object Raw(object value)
        {
            return new RawContent(value);
        }

        public override void Write(object obj = null)
        {
            object value = obj is RawContent rawContent
                ? rawContent.Value
                : System.Text.Encodings.Web.HtmlEncoder.Default.Encode(obj?.ToString() ?? string.Empty);

            base.Write(value);
        }

        public override void WriteAttributeValue(string prefix, int prefixOffset, object value, int valueOffset, int valueLength, bool isLiteral)
        {
            value = value is RawContent rawContent
                ? rawContent.Value
                : System.Text.Encodings.Web.HtmlEncoder.Default.Encode(value?.ToString() ?? string.Empty);

            base.WriteAttributeValue(prefix, prefixOffset, value, valueOffset, valueLength, isLiteral);
        }
    }
}
