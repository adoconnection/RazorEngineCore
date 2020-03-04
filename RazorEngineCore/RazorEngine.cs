using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace RazorEngineCore
{
    public class RazorEngine
    {
        public RazorEngineCompiledTemplate<T> Compile<T>(string content, Action<RazorEngineCompilationOptionsBuilder> builderAction = null) where T : RazorEngineTemplateBase
        {
            RazorEngineCompilationOptionsBuilder compilationOptionsBuilder = new RazorEngineCompilationOptionsBuilder();
            
            compilationOptionsBuilder.AddAssemblyReference(typeof(T).Assembly);
            compilationOptionsBuilder.Inherits(typeof(T));

            builderAction?.Invoke(compilationOptionsBuilder);

            MemoryStream memoryStream = this.CreateAndCompileToStream(content, compilationOptionsBuilder.Options);
           
            return new RazorEngineCompiledTemplate<T>(memoryStream);
        }

        public RazorEngineCompiledTemplate Compile(string content, Action<RazorEngineCompilationOptionsBuilder> builderAction = null)
        {
            RazorEngineCompilationOptionsBuilder compilationOptionsBuilder = new RazorEngineCompilationOptionsBuilder();
            compilationOptionsBuilder.Inherits(typeof(RazorEngineTemplateBase));
             
            builderAction?.Invoke(compilationOptionsBuilder);

            MemoryStream memoryStream = this.CreateAndCompileToStream(content, compilationOptionsBuilder.Options);

            return new RazorEngineCompiledTemplate(memoryStream);
        }

        private MemoryStream CreateAndCompileToStream(string templateSource, RazorEngineCompilationOptions options)
        {
            templateSource = this.WriteDirectives(templateSource, options);

            RazorProjectEngine engine = RazorProjectEngine.Create(
                RazorConfiguration.Default,
                RazorProjectFileSystem.Create(@"."),
                (builder) =>
                {
                    builder.SetNamespace(options.TemplateNamespace);
                });

            string fileName = Path.GetRandomFileName();

            RazorSourceDocument document = RazorSourceDocument.Create(templateSource, fileName);

            RazorCodeDocument codeDocument = engine.Process(
                document,
                null,
                new List<RazorSourceDocument>(),
                new List<TagHelperDescriptor>());

            RazorCSharpDocument razorCSharpDocument = codeDocument.GetCSharpDocument();

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(razorCSharpDocument.GeneratedCode);

            CSharpCompilation compilation = CSharpCompilation.Create(
                fileName,
                new[]
                {
                    syntaxTree
                },
                options.ReferencedAssemblies
                    .Select(ass => MetadataReference.CreateFromFile(ass.Location))
                    .ToList(),
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            MemoryStream memoryStream = new MemoryStream();

            EmitResult emitResult = compilation.Emit(memoryStream);

            if (!emitResult.Success)
            {
                List<Diagnostic> errors = emitResult.Diagnostics.ToList();

                RazorEngineCompilationException exception = new RazorEngineCompilationException("Unable to compile template: " + errors.FirstOrDefault()?.ToString());
                exception.Errors = errors;

                throw exception;
            }

            memoryStream.Position = 0;
            return memoryStream;
        }

        private string WriteDirectives(string content, RazorEngineCompilationOptions options)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("@inherits " + options.Inherits);

            foreach (string entry in options.DefaultUsings)
            {
                stringBuilder.AppendLine("@using " + entry);
            }

            stringBuilder.Append(content);

            return stringBuilder.ToString();
        }
    }
}