using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;

namespace RazorEngineCore
{
    using System;

    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class RazorEngineCompilationOptions
    {
        public HashSet<Assembly> ReferencedAssemblies { get; set; } = DefaultReferencedAssemblies();

        public HashSet<MetadataReference> MetadataReferences { get; set; } = new HashSet<MetadataReference>();
        public string TemplateNamespace { get; set; } = "TemplateNamespace";
        public string Inherits { get; set; } = "RazorEngineCore.RazorEngineTemplateBase";

        public HashSet<string> DefaultUsings { get; set; } = new HashSet<string>()
        {
            "System.Linq"
        };

        public RazorEngineCompilationOptions()
        {
            // Loading netstandard explicitly causes runtime error on Linux/OSX
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework", StringComparison.OrdinalIgnoreCase))
                {
                    this.ReferencedAssemblies.Add(
                        Assembly.Load(
                            new AssemblyName(
                                "netstandard, Version=2.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51")));
                }
                else
                {
                    this.ReferencedAssemblies.Add(Assembly.Load(new AssemblyName("netstandard")));
                }
            }
        }

        private static HashSet<Assembly> DefaultReferencedAssemblies()
        {
            if (RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework", StringComparison.OrdinalIgnoreCase))
            {
                return new HashSet<Assembly>()
                           {
                               typeof(object).Assembly,
                               Assembly.Load(new AssemblyName("Microsoft.CSharp, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")),
                               typeof(RazorEngineTemplateBase).Assembly,
                               typeof(System.Runtime.GCSettings).Assembly,
                               typeof(System.Linq.Enumerable).Assembly,
                               typeof(System.Linq.Expressions.Expression).Assembly
                           };
            }

            return new HashSet<Assembly>()
                       {
                           typeof(object).Assembly,
                           Assembly.Load(new AssemblyName("Microsoft.CSharp")),
                           typeof(RazorEngineTemplateBase).Assembly,
                           Assembly.Load(new AssemblyName("System.Runtime")),
                           Assembly.Load(new AssemblyName("System.Linq")),
                           Assembly.Load(new AssemblyName("System.Linq.Expressions"))
                       };
        }
    }
}