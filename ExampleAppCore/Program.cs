using RazorEngineCore;
using System;
using System.Collections.Generic;
namespace ExampleApp
{
    public class TestModel : RazorEngineTemplateBase
    {
        public string Name { get; set; }
        public IEnumerable<int> Items { get; set; }
    }


    class Program
    {
        static string Content = @"
@(""Hello"" + Model.Name)111

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
            RegularExample();
            CachedHtmlSafeExample();

            Console.ReadKey();
        }


        static void RegularExample()
        {
            IRazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate template = razorEngine.Compile(Content);

            string result = template.Run(new
            {
                Name = "<b>Alex</b>",
                Items = new List<string>()
                {
                    "item 1",
                    "item 2"
                }
            });

            Console.WriteLine(result);
        }

        static void CachedHtmlSafeExample()
        {
            string result = RazorTemplateCache.RenderHtmlSafe("Hello <h1>@Model.Name</h1>", new { Name = "<b>Alex</b>" });
            Console.WriteLine(result);
        }
    }
}
