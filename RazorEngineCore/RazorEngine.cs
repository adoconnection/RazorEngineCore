using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System.Reflection.Metadata;

namespace RazorEngineCore
{
    public class RazorEngine : IRazorEngine
    {
        public IRazorEngineCompiledTemplate<T> Compile<T>(string content, Action<IRazorEngineCompilationOptionsBuilder> builderAction = null, CancellationToken cancellationToken = default) where T : IRazorEngineTemplate
        {
            IRazorEngineCompilationOptionsBuilder compilationOptionsBuilder = new RazorEngineCompilationOptionsBuilder();
            compilationOptionsBuilder.AddAssemblyReference(typeof(T).Assembly);
            compilationOptionsBuilder.Inherits(typeof(T));

            builderAction?.Invoke(compilationOptionsBuilder);

            RazorEngineCompiledTemplateMeta meta = this.CreateAndCompileToStream(content, compilationOptionsBuilder.Options, cancellationToken);

            return new RazorEngineCompiledTemplate<T>(meta);
        }

        public Task<IRazorEngineCompiledTemplate<T>> CompileAsync<T>(string content, Action<IRazorEngineCompilationOptionsBuilder> builderAction = null, CancellationToken cancellationToken = default) where T : IRazorEngineTemplate
        {
            return Task.Run(() => this.Compile<T>(content: content, builderAction: builderAction, cancellationToken: cancellationToken));
        }

        public IRazorEngineCompiledTemplate Compile(string content, Action<IRazorEngineCompilationOptionsBuilder> builderAction = null, CancellationToken cancellationToken = default)
        {
            IRazorEngineCompilationOptionsBuilder compilationOptionsBuilder = new RazorEngineCompilationOptionsBuilder();
            compilationOptionsBuilder.Inherits(typeof(RazorEngineTemplateBase));

            builderAction?.Invoke(compilationOptionsBuilder);
 
            RazorEngineCompiledTemplateMeta meta = this.CreateAndCompileToStream(content, compilationOptionsBuilder.Options, cancellationToken);
            return new RazorEngineCompiledTemplate(meta);
        }

        public Task<IRazorEngineCompiledTemplate> CompileAsync(string content, Action<IRazorEngineCompilationOptionsBuilder> builderAction = null, CancellationToken cancellationToken = default)
        {
            return Task.Run(() => this.Compile(
                content, 
                builderAction, 
                cancellationToken));
        }

        protected virtual RazorEngineCompiledTemplateMeta CreateAndCompileToStream(string templateSource, RazorEngineCompilationOptions options, CancellationToken cancellationToken)
        {
            templateSource = this.WriteDirectives(templateSource, options);

            string projectPath = @".";
            string fileName = string.IsNullOrWhiteSpace(options.TemplateFilename) 
                ? Path.GetRandomFileName() + ".cshtml" 
                : options.TemplateFilename;

            RazorProjectEngine engine = RazorProjectEngine.Create(
                RazorConfiguration.Default,
                RazorProjectFileSystem.Create(projectPath),
                (builder) =>
                {
                    builder.SetNamespace(options.TemplateNamespace);
                    options.ProjectEngineBuilderAction?.Invoke(builder);
                });

            RazorSourceDocument document = RazorSourceDocument.Create(templateSource, fileName);

            RazorCodeDocument codeDocument = engine.Process(
                document,
                null,
                new List<RazorSourceDocument>(),
                new List<TagHelperDescriptor>());
            
            RazorCSharpDocument razorCSharpDocument = codeDocument.GetCSharpDocument();
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(razorCSharpDocument.GeneratedCode, cancellationToken: cancellationToken);

            CSharpCompilation compilation = CSharpCompilation.Create(
                fileName,
                new[]
                {
                    syntaxTree
                },
                options.ReferencedAssemblies
                   .Select(ass =>
                   {
#if NETSTANDARD2_0
                            return  MetadataReference.CreateFromFile(ass.Location); 
#else
                       unsafe
                       {
                           ass.TryGetRawMetadata(out byte* blob, out int length);
                           ModuleMetadata moduleMetadata = ModuleMetadata.CreateFromMetadata((IntPtr)blob, length);
                           AssemblyMetadata assemblyMetadata = AssemblyMetadata.Create(moduleMetadata);
                           PortableExecutableReference metadataReference = assemblyMetadata.GetReference();

                           return metadataReference;
                       }
#endif
                   })
                    .Concat(options.MetadataReferences)
                    .ToList(),
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                    .WithOptimizationLevel(OptimizationLevel.Release)
                    .WithOverflowChecks(true));


            MemoryStream assemblyStream = new MemoryStream();
            MemoryStream pdbStream = options.IncludeDebuggingInfo ? new MemoryStream() : null;

            EmitResult emitResult = compilation.Emit(assemblyStream, pdbStream, cancellationToken: cancellationToken);

            if (!emitResult.Success)
            {
                RazorEngineCompilationException exception = new RazorEngineCompilationException()
                {
                    Errors = emitResult.Diagnostics.ToList(),
                    GeneratedCode = razorCSharpDocument.GeneratedCode
                };

                throw exception;
            }

            return new RazorEngineCompiledTemplateMeta()
            {
                AssemblyByteCode = assemblyStream.ToArray(),
                PdbByteCode = pdbStream?.ToArray(),
                GeneratedSourceCode = options.IncludeDebuggingInfo ? razorCSharpDocument.GeneratedCode : null,
                TemplateSource = options.IncludeDebuggingInfo ? templateSource : null,
                TemplateNamespace = options.TemplateNamespace,
                TemplateFileName = fileName
            };
        }

        protected virtual string WriteDirectives(string content, RazorEngineCompilationOptions options)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"@inherits {options.Inherits}");

            foreach (string entry in options.DefaultUsings)
            {
                stringBuilder.AppendLine($"@using {entry}");
            }

            stringBuilder.Append(content);

            return stringBuilder.ToString();
        }
    }
}