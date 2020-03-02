using System;
using System.Collections.Generic;
using RazorEngineCore;

namespace ExampleApp
{
    class Program
    {
        static string Content = @"
Hello @Model.Name

@foreach(var item in @Model.Items)
{
    <div>- @item</div>
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
			RazorEngine razorEngine = new RazorEngine();
            RazorEngineCompiledTemplate template = razorEngine.Compile(Content);

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
