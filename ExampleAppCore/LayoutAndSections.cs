using Microsoft.AspNetCore.Razor.Language.Extensions;
using RazorEngineCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;


namespace ExampleApp
{
 
    public static class LayoutAndSections
    {
        /// <summary>
        /// Provides an example of using @section and @RenderSection, based on the layout implementation
        /// at https://github.com/adoconnection/RazorEngineCore/wiki/@Include-and-@Layout
        /// </summary>
        public static void Test()
        {

            string template = @"
                @{
                    Layout = ""MyLayout"";
                 }

                <h1>@Model.Title</h1>

                @Include(""outer"", Model)

                @section mysection 
                {
                    This is in a section
                }
            ";

            IDictionary<string, string> parts = new Dictionary<string, string>()
            {
                    {"MyLayout", @"
                        LAYOUT HEADER
                            @RenderBody()
                            @RenderSection(""mysection"")
                        LAYOUT FOOTER
                    "},
                    {"outer", "This is Outer include, <@Model.Title>, @Include(\"inner\")"},
                    {"inner", "This is Inner include"}
            };

            var razorEngine = new RazorEngine();

            MyCompiledTemplate compiledTemplate = razorEngine.Compile(template, parts, (builder) =>
            {
                builder.Options.ProjectEngineBuilderAction = (x) => SectionDirective.Register(x);
            });

            string result = compiledTemplate.Run(new { Title = "Hello" });

            Console.WriteLine(result);
            Console.ReadKey();
        }
    }


    public class MyTemplateBase : RazorEngineTemplateBase
    {
        public Func<string, object, string> IncludeCallback { get; set; }
        public Func<string> RenderBodyCallback { get; set; }
        public string Layout { get; set; }

        public string Include(string key, object model = null)
        {
            return this.IncludeCallback(key, model);
        }

        public string RenderBody()
        {
            return this.RenderBodyCallback();
        }

        public Dictionary<string, string> Sections = new Dictionary<string, string>();

        public void DefineSection(string name, Action a)
        {
            Debug.WriteLine("DefineSection " + name);
            var sb = new StringBuilder();
            BeginCapture(sb);
            a();
            EndCapture();
            string content = sb.ToString();
            Sections.Add(name, content);
            Debug.WriteLine("Section content:");
            Debug.WriteLine(content);
        }

        public string RenderSection(string name, bool required = false)
        {
            if (Sections.TryGetValue(name, out var content))
                return content;
            else if (required)
                throw new Exception("Required section not found: " + name);
            return string.Empty;
        }
    }


    public class MyCompiledTemplate
    {
        private readonly IRazorEngineCompiledTemplate<MyTemplateBase> compiledTemplate;
        private readonly Dictionary<string, IRazorEngineCompiledTemplate<MyTemplateBase>> compiledParts;

        public MyCompiledTemplate(IRazorEngineCompiledTemplate<MyTemplateBase> compiledTemplate, Dictionary<string, IRazorEngineCompiledTemplate<MyTemplateBase>> compiledParts)
        {
            this.compiledTemplate = compiledTemplate;
            this.compiledParts = compiledParts;
        }

        public string Run(object model)
        {
            return this.Run(this.compiledTemplate, model);
        }

        public string Run(IRazorEngineCompiledTemplate<MyTemplateBase> template, object model)
        {
            MyTemplateBase templateReference = null;

            string result = template.Run(instance =>
            {
                if (!(model is AnonymousTypeWrapper))
                {
                    model = new AnonymousTypeWrapper(model);
                }

                instance.Model = model;
                instance.IncludeCallback = (key, includeModel) => this.Run(this.compiledParts[key], includeModel);

                templateReference = instance;
            });

            if (templateReference.Layout == null)
            {
                return result;
            }

            return this.compiledParts[templateReference.Layout].Run(instance =>
            {
                if (!(model is AnonymousTypeWrapper))
                {
                    model = new AnonymousTypeWrapper(model);
                }
                instance.Sections = templateReference.Sections;
                instance.Model = model;
                instance.IncludeCallback = (key, includeModel) => this.Run(this.compiledParts[key], includeModel);
                instance.RenderBodyCallback = () => result;
            });
        }

        public void Save()
        {
            /*
            TODO

            this.compiledTemplate.SaveToFile();
            this.compiledTemplate.SaveToStream();

            foreach (var compiledPart in this.compiledParts)
            {
                compiledPart.Value.SaveToFile();
                compiledPart.Value.SaveToStream();
            }
            */
        }

        public void Load()
        {
            // TODO
        }
    }

}
