using System.Text.Encodings.Web;
using System.Text.Unicode;
using RazorEngineCore;

namespace ExampleAppNET5
{
    public class HtmlSafeTemplate : RazorEngineTemplateBase
    {
        // HtmlEncoder.Default escapes everything outside Basic Latin (Cyrillic, CJK etc.) as &#x...;
        private static readonly HtmlEncoder Encoder = HtmlEncoder.Create(UnicodeRanges.All);

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
                : Encoder.Encode(obj?.ToString() ?? string.Empty);

            base.Write(value);
        }

        public override void WriteAttributeValue(string prefix, int prefixOffset, object value, int valueOffset, int valueLength, bool isLiteral)
        {
            // literal parts of attribute come from the template as is and may already contain entities like &amp;
            if (!isLiteral)
            {
                value = value is RawContent rawContent
                    ? rawContent.Value
                    : Encoder.Encode(value?.ToString() ?? string.Empty);
            }

            base.WriteAttributeValue(prefix, prefixOffset, value, valueOffset, valueLength, isLiteral);
        }
    }
}
