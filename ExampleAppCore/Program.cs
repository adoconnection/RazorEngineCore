using System;
using System.Collections.Generic;
using System.Text;
using RazorEngineCore;

namespace ExampleApp
{
    public class TestModel : RazorEngineTemplateBase
    {
        public string Name { get; set; }
        public IEnumerable<int> Items { get; set; }
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
    @myitem
}


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
            IRazorEngineCompiledTemplate<string> template = razorEngine.Compile(Content);

            /*
            StringBuilder stringBuilder = new StringBuilder();
            byte[] somebytes = { 0x02, 0x04, 0x50 };
            stringBuilder.Append(somebytes);

            var s = stringBuilder.ToString();
            */

            string result = template.Run(new
            {
                Name = "Alexander",
                Items = new List<string>()
                {
                    "item 1",
                    "item 2"
                }
            });

            Console.WriteLine(result);
            Console.ReadKey();
        }
    }
}
