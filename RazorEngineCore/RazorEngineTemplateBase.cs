using System.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    public abstract class RazorEngineTemplateBase
    {
        private readonly StringBuilder stringBuilder = new StringBuilder();
        public dynamic Model { get; set; }

        public void WriteLiteral(string literal)
        {
            this.stringBuilder.Append(literal);
        }

        public void Write(object obj)
        {
            this.stringBuilder.Append(obj);
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