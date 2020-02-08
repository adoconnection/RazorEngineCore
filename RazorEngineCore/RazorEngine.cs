using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace RazorEngineCore
{
    public class RazorEngine
    {
        public RazorEngineCompiledTemplate Compile(string content)
        {
            RazorProjectEngine engine = RazorProjectEngine.Create(
                RazorConfiguration.Default,
                RazorProjectFileSystem.Create(@"."),
                (builder) =>
                {
                    builder.SetNamespace("TemplateNamespace");
                }); 

            string fileName = Path.GetRandomFileName();

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("@inherits RazorEngineCore.RazorEngineTemplateBase");
            stringBuilder.AppendLine(content);

            RazorSourceDocument document = RazorSourceDocument.Create(stringBuilder.ToString(), fileName);

            RazorCodeDocument codeDocument = engine.Process(
                document,
                null,
                new List<RazorSourceDocument>(),
                new List<TagHelperDescriptor>() { });

            RazorCSharpDocument razorCSharpDocument = codeDocument.GetCSharpDocument();

            CSharpCompilation compilation = CSharpCompilation.Create(
                fileName,
                new[]
                {
                    CSharpSyntaxTree.ParseText(razorCSharpDocument.GeneratedCode)
                },
                new[]
                {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.CSharp")).Location),
                    MetadataReference.CreateFromFile(typeof(RazorEngineTemplateBase).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(ExpandoObject).Assembly.Location),
                    MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("netstandard")).Location),
                    MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Runtime")).Location),
                },
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            MemoryStream memoryStream = new MemoryStream();

            EmitResult emitResult = compilation.Emit(memoryStream);

            if (!emitResult.Success)
            {
                RazorEngineCompilationException exception = new RazorEngineCompilationException("Unable to compile template");
                exception.Errors = emitResult.Diagnostics.ToList();

                throw exception;
            }

            memoryStream.Position = 0;

            return new RazorEngineCompiledTemplate(memoryStream);
        }

    }
}