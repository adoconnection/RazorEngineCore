#if !NETSTANDARD2_0
using System;
using System.Reflection;
using System.Runtime.Loader;

namespace RazorEngineCore
{
    internal class RazorTemplateLoadContext : AssemblyLoadContext
    {
        // Empty in single-file apps, then the shared Microsoft.CSharp is used and dynamic templates won't unload
        private static readonly string MicrosoftCSharpPath = typeof(Microsoft.CSharp.RuntimeBinder.Binder).Assembly.Location;

        public RazorTemplateLoadContext() : base(isCollectible: true)
        {
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            // Microsoft.CSharp keeps static caches of binders that root the template type,
            // a private copy lets those caches be collected together with the context
            // https://github.com/dotnet/runtime/issues/71629
            if (string.Equals(assemblyName.Name, "Microsoft.CSharp", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(MicrosoftCSharpPath))
            {
                return this.LoadFromAssemblyPath(MicrosoftCSharpPath);
            }

            // null = resolve the rest (RazorEngineCore, user model types) from Default context,
            // so IRazorEngineTemplate and model types stay the same types on both sides
            return null;
        }
    }
}
#endif
