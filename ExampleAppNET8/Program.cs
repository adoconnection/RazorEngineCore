using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using RazorEngineCore;

namespace ExampleAppNET5
{

    class Program
    {
        static void Main(string[] args)
        {
            IRazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate template1 = razorEngine.Compile("Hello <h1>@Model.Name</h1>");

            string result = template1.Run(new
            {
	            Name = "<b>Alex</b>"
			});

            Console.WriteLine(result);
        }


        static void ReadFromFileAndRun()
        {
            IRazorEngine razorEngine = new RazorEngine();
            string templateText = File.ReadAllText("template2.cshtml");

            IRazorEngineCompiledTemplate template2 = razorEngine.Compile(templateText, builder =>
            {
                builder.IncludeDebuggingInfo();
            });

            if (!Directory.Exists("Temp"))
            {
                Directory.CreateDirectory("Temp");
            }

            template2.EnableDebugging("Temp");

            string result = template2.Run(new
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
