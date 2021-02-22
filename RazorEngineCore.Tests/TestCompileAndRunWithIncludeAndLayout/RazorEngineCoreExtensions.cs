using System.Collections.Generic;
using System.Linq;

namespace RazorEngineCore.Tests
{
    public static class RazorEngineCoreExtensions
    {
        public static RazorCustomizedCompiledTemplate Compile(this RazorEngine razorEngine, string template, IDictionary<string, string> parts)
        {
            return new RazorCustomizedCompiledTemplate(
                    razorEngine.Compile<RazorIncludeAndLayoutTemplateBase>(template),
                    parts.ToDictionary(
                            k => k.Key,
                            v => razorEngine.Compile<RazorIncludeAndLayoutTemplateBase>(v.Value)));
        }
    }
}