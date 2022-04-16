using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;
using RazorEngineCore;

namespace ExampleApp
{
    public abstract class BinaryDataEngineTemplateBase<T> : BinaryDataEngineTemplateBase
    {
        public new T Model { get; set; }
    }

    public class BinaryDataEngineTemplateBase : IRazorEngineTemplate<byte[]>
    {
        readonly MemoryStream binaryWriter = new();

        Encoding encoding = Encoding.UTF8;

        private string attributeSuffix = null;

        public dynamic Model { get; set; }

        public void WriteLiteral(string literal = null)
        {
            WriteLiteralAsync(literal).GetAwaiter().GetResult();
        }

        public virtual Task WriteLiteralAsync(string literal = null)
        {
            Write(literal);
            return Task.CompletedTask;
        }

        public void Write(object obj = null)
        {
            WriteAsync(obj).GetAwaiter().GetResult();
        }

        public virtual Task WriteAsync(object obj = null)
        {
            if (obj is byte[] bytes)
                binaryWriter.Write(bytes);
            else
                binaryWriter.Write( encoding.GetBytes(obj.ToString()));
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
            Write(prefix);
            return Task.CompletedTask;
        }

        public void WriteAttributeValue(string prefix, int prefixOffset, object value, int valueOffset, int valueLength,
            bool isLiteral)
        {
            WriteAttributeValueAsync(prefix, prefixOffset, value, valueOffset, valueLength, isLiteral).GetAwaiter().GetResult();
        }

        public virtual Task WriteAttributeValueAsync(string prefix, int prefixOffset, object value, int valueOffset, int valueLength, bool isLiteral)
        {
            Write(prefix);
            Write(value);
            return Task.CompletedTask;
        }

        public void EndWriteAttribute()
        {
            EndWriteAttributeAsync().GetAwaiter().GetResult();
        }

        public virtual Task EndWriteAttributeAsync()
        {
            Write(attributeSuffix);
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

        public virtual byte[] Result()
        {
            return ResultAsync().GetAwaiter().GetResult();
        }

        public virtual Task<byte[]> ResultAsync()
        {
            return Task.FromResult<byte[]>( binaryWriter.ToArray() );
        }
    }

    public class TestModel : BinaryDataEngineTemplateBase<TestModel>
    {
        public string Name { get; set; }
        public IEnumerable<string> Items { get; set; }
        public byte[] BinaryData { get; set; }
    }



    class Program
    {
        static readonly string Content = @"
Hello @Model.Name

@{
    string testheader = "" ---------------- mytest: will show the modified items ----------- "";
    List<string> myitems = new ();
    foreach (var item in Model.Items) {
        myitems.Add("" ---->>>> "" + item);
    }
}

@foreach(var item in @Model.Items)
{
    <div>- @item</div>
}
@testheader
@foreach(var myitem in @myitems)
{
   <text>|  @(myitem)   |  @(myitem)    |   @(myitem)   |
</text>
}
Binary data below:
@Model.BinaryData
End of Binary data

<div data-name=""@Model.Name""></div>

<area>
    @{ RecursionTest(3); }
</area>

@{
	void RecursionTest(int level){
		if (level <= 0)
		{
			return;
		}
			
		<div>LEVEL: @level</div>
		@{ RecursionTest(level - 1); }
	}
}";

        static void Main(string[] args)
        {
            IRazorEngine razorEngine = new RazorEngine();
            var template = razorEngine.Compile<BinaryDataEngineTemplateBase<TestModel>, byte[]>(Content);

            /*
            StringBuilder stringBuilder = new StringBuilder();
            byte[] somebytes = { 0x02, 0x04, 0x50 };
            stringBuilder.Append(somebytes);
            
            var s = stringBuilder.ToString();
            */

            TestModel model = new TestModel()
            {
                Name = "Alexander",
                Items = new List<string>()
                {
                    "item 1",
                    "item 2"
                },
                BinaryData = new byte[10] { 0x41, 0x0A, 0x0D, 0x00, 0x30, 0x31, 0x00, 0x0A, 0x0D, 0x41 } 
            };



            byte[] result = template.Run(instance => instance.Model = model);

            Console.WriteLine(Encoding.UTF8.GetString(result));
            Console.ReadKey();
        }
    }
}
