using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;

namespace RazorEngineCore
{
    public class RazorEngineCompilationOptionsBuilder : IRazorEngineCompilationOptionsBuilder
    {
        public RazorEngineCompilationOptions Options { get; set; }

        public RazorEngineCompilationOptionsBuilder(RazorEngineCompilationOptions options = null)
        {
            this.Options = options ?? new RazorEngineCompilationOptions();
        }

        /// <summary>
        /// Loads the Assembly by name and adds it to the Engine's Assembly Reference list
        /// </summary>
        /// <param name="assemblyName">Full Name of the Assembly to add to the assembly list</param>
        public void AddAssemblyReferenceByName(string assemblyName)
        {
            Assembly assembly = Assembly.Load(new AssemblyName(assemblyName));
            this.AddAssemblyReference(assembly);
        }

        /// <summary>
        /// Adds a loaded assembly to the Engine's Assembly Reference list
        /// </summary>
        /// <param name="assembly">Assembly to add to the assembly list</param>
        public void AddAssemblyReference(Assembly assembly)
        {
            this.Options.ReferencedAssemblies.Add(assembly);
        }

        /// <summary>
        /// <para>Adds a type's assembly to the Engine's Assembly Reference list</para>
        /// <para>Also adds the type's GenericTypeArguments to the Reference list as well</para>
        /// </summary>
        /// <param name="type">The type who's assembly should be added to the assembly list</param>
        public void AddAssemblyReference(Type type)
        {
            this.AddAssemblyReference(type.Assembly);

            foreach (Type argumentType in type.GenericTypeArguments)
            {
                this.AddAssemblyReference(argumentType);
            }
        }

        /// <summary>
        /// Adds a MetadataReference for use in the Engine's Assembly Reference generation
        /// </summary>
        /// <param name="reference">Metadata Reference to add to the Engine's Referenced Assemblies</param>
        public void AddMetadataReference(MetadataReference reference)
        {
            this.Options.MetadataReferences.Add(reference);
        }

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
        public void AddUsing(string namespaceName)
        {
            this.Options.DefaultUsings.Add(namespaceName);
        }

        /// <summary>
        /// Adds type to @inherit from in the template
        /// </summary>
        /// <param name="type">Type to @inherit from</param>
        public void Inherits(Type type)
        {
            this.Options.Inherits = this.RenderTypeName(type);
            this.AddAssemblyReference(type);
        }

        private string RenderTypeName(Type type)
        {
            IList<string> elements = new List<string>()
            {
                type.Namespace,
                RenderDeclaringType(type.DeclaringType),
                type.Name
            };

            string result = string.Join(".", elements.Where(e => !string.IsNullOrWhiteSpace(e)));

            int tildeLocation = result.IndexOf('`');
            if (tildeLocation > -1)
            {
                result = result.Substring(0, tildeLocation);
            }

            if (type.GenericTypeArguments.Length == 0)
            {
                return result;
            }

            return result + "<" + string.Join(",", type.GenericTypeArguments.Select(this.RenderTypeName)) + ">";
        }

        private string RenderDeclaringType(Type type)
        {
            if (type == null)
            {
                return null;
            }

            string parent = RenderDeclaringType(type.DeclaringType);

            if (string.IsNullOrWhiteSpace(parent))
            {
                return type.Name;
            }

            return parent + "." + type.Name;
        }

        /// <summary>
        /// Enables debug info
        /// </summary>
        public void IncludeDebuggingInfo()
        {
            this.Options.IncludeDebuggingInfo = true;
        }

    }
}
