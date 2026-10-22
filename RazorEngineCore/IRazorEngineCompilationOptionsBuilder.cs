using System;
using System.Reflection;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.CodeAnalysis;

namespace RazorEngineCore
{
    public interface IRazorEngineCompilationOptionsBuilder
    {
        RazorEngineCompilationOptions Options { get; set; }
        
        /// <summary>
        /// Loads the Assembly by name and adds it to the Engine's Assembly Reference list
        /// </summary>
        /// <param name="assemblyName">Full Name of the Assembly to add to the assembly list</param>
        void AddAssemblyReferenceByName(string assemblyName);

        /// <summary>
        /// Adds a loaded assembly to the Engine's Assembly Reference list
        /// </summary>
        /// <param name="assembly">Assembly to add to the assembly list</param>
        void AddAssemblyReference(Assembly assembly);

        /// <summary>
        /// <para>Adds a type's assembly to the Engine's Assembly Reference list</para>
        /// <para>Also adds the type's GenericTypeArguments to the Reference list as well</para>
        /// </summary>
        /// <param name="type">The type who's assembly should be added to the assembly list</param>
        void AddAssemblyReference(Type type);

        /// <summary>
        /// Adds a MetadataReference for use in the Engine's Assembly Reference generation
        /// </summary>
        /// <param name="reference">Metadata Reference to add to the Engine's Referenced Assemblies</param>
        void AddMetadataReference(MetadataReference reference);
        
        /// <summary>
            /// <para>Adds a default <c>using</c> to the compiled view. This is equivalent to adding <c>@using [NAMESPACE]</c> to every template rendered by the engine'</para>
            /// Current Defaults: 
            /// <list type="bullet">
            ///     <listheader></listheader>
            ///     <item>System.Linq</item>
            ///     <item>System.Collections</item>
            ///     <item>System.Collections.Generic</item>
            /// </list>
        /// </summary>
        /// <param name="namespaceName">Namespace to add to default usings</param>
        void AddUsing(string namespaceName);
        
        /// <summary>
        /// Adds <c>@inherits</c> directive to the compiled template
        /// </summary>
        /// <param name="type">Type to <c>@inherits</c> from</param>
        void Inherits(Type type);

        /// <summary>
        /// Enables debug info
        /// </summary>
        void IncludeDebuggingInfo();

        /// <summary>
        /// <para>Loads the compiled template into its own collectible AssemblyLoadContext, so it is unloaded on Dispose() or when garbage collected.</para>
        /// <para>Use when compiling many unique templates in a long running process. First Run() of each template is significantly slower,
        /// because the template gets its own copy of Microsoft.CSharp (see https://github.com/dotnet/runtime/issues/71629).</para>
        /// <para>.NET 5+ only, ignored on netstandard2.0. Templates using dynamic are not unloaded in single-file apps.</para>
        /// </summary>
        void EnableUnloading();

        /// <summary>
        /// Access RazorProjectEngineBuilder to register new Features etc
        /// </summary>
        /// <param name="configure"></param>
        void ConfigureRazorEngineProject(Action<RazorProjectEngineBuilder> configure);
    }
}