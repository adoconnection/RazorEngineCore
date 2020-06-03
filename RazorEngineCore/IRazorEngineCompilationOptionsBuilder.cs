using System;
using System.Reflection;

namespace RazorEngineCore
{
    public interface IRazorEngineCompilationOptionsBuilder
    {
        RazorEngineCompilationOptions Options { get; set; }
        
        void AddAssemblyReferenceByName(string assemblyName);
        
        void AddAssemblyReference(Assembly assembly);
        
        void AddAssemblyReference(Type type);
        
        void AddUsing(string namespaceName);
        
        void Inherits(Type type);
    }
}