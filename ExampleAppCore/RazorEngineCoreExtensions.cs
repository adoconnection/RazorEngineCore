using RazorEngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExampleApp
{
    public static class RazorEngineCoreExtensions
    {
        public static MyCompiledTemplate Compile(this RazorEngine razorEngine, string template, IDictionary<string, string> parts,
            Action<IRazorEngineCompilationOptionsBuilder> builderAction = null)
        {
            return new MyCompiledTemplate(
                    razorEngine.Compile<MyTemplateBase>(template, builderAction),
                    parts.ToDictionary(
                            k => k.Key,
                            v => razorEngine.Compile<MyTemplateBase>(v.Value)));
        }

    }

}
