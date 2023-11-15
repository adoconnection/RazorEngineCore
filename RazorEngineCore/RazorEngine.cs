using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        public IRazorEngineCompiledTemplate<T> Compile<T>(string content, Action<IRazorEngineCompilationOptionsBuilder> builderAction = null) where T : IRazorEngineTemplate
        {
            IRazorEngineCompilationOptionsBuilder compilationOptionsBuilder = new RazorEngineCompilationOptionsBuilder();
            compilationOptionsBuilder.AddAssemblyReference(typeof(T).Assembly);
            compilationOptionsBuilder.Inherits(typeof(T));

            builderAction?.Invoke(compilationOptionsBuilder);
            if (compilationOptionsBuilder.Options.GeneratePdbStream)
            {
                CompiledStreams streams = this.CreateAndCompileToStream(content, compilationOptionsBuilder.Options);
                return new RazorEngineCompiledTemplate<T>(streams.assembly, compilationOptionsBuilder.Options.TemplateNamespace, streams.pdb);
            }
            else
            {
                CompiledStreams streams = this.CreateAndCompileToStream(content, compilationOptionsBuilder.Options);
                return new RazorEngineCompiledTemplate<T>(streams.assembly, compilationOptionsBuilder.Options.TemplateNamespace);
            }
        }

        public Task<IRazorEngineCompiledTemplate<T>> CompileAsync<T>(string content, Action<IRazorEngineCompilationOptionsBuilder> builderAction = null) where T : IRazorEngineTemplate
        {
            return Task.Factory.StartNew(() => this.Compile<T>(content: content, builderAction: builderAction));
        }

        public IRazorEngineCompiledTemplate Compile(string content, Action<IRazorEngineCompilationOptionsBuilder> builderAction = null)
        {
            IRazorEngineCompilationOptionsBuilder compilationOptionsBuilder = new RazorEngineCompilationOptionsBuilder();
            compilationOptionsBuilder.Inherits(typeof(RazorEngineTemplateBase));

            builderAction?.Invoke(compilationOptionsBuilder);
            if (compilationOptionsBuilder.Options.GeneratePdbStream)
            {
                CompiledStreams streams = this.CreateAndCompileToStream(content, compilationOptionsBuilder.Options);
                return new RazorEngineCompiledTemplate(streams.assembly, compilationOptionsBuilder.Options.TemplateNamespace, streams.pdb);
            }
            else
            {
                CompiledStreams streams = this.CreateAndCompileToStream(content, compilationOptionsBuilder.Options);
                return new RazorEngineCompiledTemplate(streams.assembly, compilationOptionsBuilder.Options.TemplateNamespace);
            }

        }

        public Task<IRazorEngineCompiledTemplate> CompileAsync(string content, Action<IRazorEngineCompilationOptionsBuilder> builderAction = null)
        {
            return Task.Factory.StartNew(() => this.Compile(content: content, builderAction: builderAction));
        }

        protected virtual CompiledStreams CreateAndCompileToStream(string templateSource, RazorEngineCompilationOptions options, MemoryStream pdbStream = null)
        {
            templateSource = this.WriteDirectives(templateSource, options);
            string projectPath = @".";
            string fileName = string.IsNullOrWhiteSpace(options.TemplateFilename) ? Path.GetRandomFileName() + ".cshtml" : options.TemplateFilename;

            if (options.GeneratePdbStream)
            {
                projectPath = Path.GetTempPath();
                Directory.CreateDirectory(projectPath);
                File.WriteAllText(Path.Combine(projectPath, fileName), templateSource);
            }

            RazorProjectEngine engine = RazorProjectEngine.Create(
                RazorConfiguration.Default,
                RazorProjectFileSystem.Create(projectPath),
                (builder) =>
                {
                    builder.SetNamespace(options.TemplateNamespace);
                });


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
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            CompiledStreams streams = new CompiledStreams();
            EmitResult emitResult;
            if (options.GeneratePdbStream)
                emitResult = compilation.Emit(streams.assembly, streams.pdb);
            else
                emitResult = compilation.Emit(streams.assembly);

            if (!emitResult.Success)
            {
                RazorEngineCompilationException exception = new RazorEngineCompilationException()
                {
                    Errors = emitResult.Diagnostics.ToList(),
                    GeneratedCode = razorCSharpDocument.GeneratedCode
                };

                throw exception;
            }

            streams.assembly.Position = 0;
            if(options.GeneratePdbStream)
                streams.pdb.Position = 0;
            return streams;
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

        protected class CompiledStreams
        {
            public CompiledStreams()
            {
                assembly = new MemoryStream();
                pdb = new MemoryStream();
            }
            public MemoryStream assembly {get;set;}
            public MemoryStream pdb {get;set;}
        }
    }
}
