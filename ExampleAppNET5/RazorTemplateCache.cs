using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RazorEngineCore;

namespace ExampleAppNET5
{
    public static class RazorTemplateCache
    {
        private static readonly ConcurrentDictionary<string, object> Cache = new(StringComparer.Ordinal);

        public static string Render(string template, object? model = null)
        {
            IRazorEngineCompiledTemplate compiledTemplate = (IRazorEngineCompiledTemplate)Cache.GetOrAdd(template, i =>
            {
                IRazorEngine razorEngine = new RazorEngine();
                return razorEngine.Compile(template);
            });

            return compiledTemplate.Run(model);
        }

        public static string RenderHtmlSafe(string template, object? model = null)
        {
            return Render<HtmlSafeTemplate>(template, model);   
        }


        public static string Render<T>(string template, object? model = null) where T : IRazorEngineTemplate
        {
            IRazorEngineCompiledTemplate<T> compiledTemplate = (IRazorEngineCompiledTemplate<T>)Cache.GetOrAdd(template, i =>
            {
                IRazorEngine razorEngine = new RazorEngine();
                return razorEngine.Compile<T>(template);
            });

            if (model.IsAnonymous())
            {
                model = new AnonymousTypeWrapper(model);
            }

            return compiledTemplate.Run(instance =>
            {
                instance.Model = model;
            });
        }
    }
}
