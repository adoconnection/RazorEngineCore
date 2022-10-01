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

        public void AddAssemblyReferenceByName(string assemblyName)
        {
            Assembly assembly = Assembly.Load(new AssemblyName(assemblyName));
            this.AddAssemblyReference(assembly);
        }

        public void AddAssemblyReference(Assembly assembly)
        {
            this.Options.ReferencedAssemblies.Add(assembly);
        }

        public void AddAssemblyReference(Type type)
        {
            this.AddAssemblyReference(type.Assembly);

            foreach (Type argumentType in type.GenericTypeArguments)
            {
                this.AddAssemblyReference(argumentType);
            }
        }

        public void AddMetadataReference(MetadataReference reference)
        {
            this.Options.MetadataReferences.Add(reference);
        }

        public void AddUsing(string namespaceName)
        {
            this.Options.DefaultUsings.Add(namespaceName);
        }

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

            if (result.Contains('`'))
            {
                result = result.Substring(0, result.IndexOf("`"));
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
    }
}