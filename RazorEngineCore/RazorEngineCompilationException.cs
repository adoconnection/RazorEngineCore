using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.CodeAnalysis;

namespace RazorEngineCore
{
    public class RazorEngineCompilationException : RazorEngineException
    {
        public RazorEngineCompilationException()
        {
        }

        protected RazorEngineCompilationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public RazorEngineCompilationException(Exception innerException) : base(null, innerException)
        {
        }

        public List<Diagnostic> Errors { get; set; }
        
        public string GeneratedCode { get; set; }

        public override string Message => $"Unable to compile template: {string.Join("\n", Errors.Where(w => w.IsWarningAsError || w.Severity == DiagnosticSeverity.Error))}";
    }
}