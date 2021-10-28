﻿using System;
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

            MemoryStream memoryStream = this.CreateAndCompileToStream(content, compilationOptionsBuilder.Options);

            return new RazorEngineCompiledTemplate<T>(memoryStream);
        }

        public Task<IRazorEngineCompiledTemplate<T>> CompileAsync<T>(string content, Action<IRazorEngineCompilationOptionsBuilder> builderAction = null) where T : IRazorEngineTemplate
        {
            return Task.Factory.StartNew(() => this.Compile<T>(content: content, builderAction: builderAction));
        }
        public IRazorEngineCompiledTemplate Compile(string content, Action<IRazorEngineCompilationOptionsBuilder> builderAction = null)
        {
            return Compile(content,builderAction,false);
        }
        public IRazorEngineCompiledTemplate Compile(string content, Action<IRazorEngineCompilationOptionsBuilder> builderAction = null, bool addPdb = false)
        {
            IRazorEngineCompilationOptionsBuilder compilationOptionsBuilder = new RazorEngineCompilationOptionsBuilder();
            compilationOptionsBuilder.Inherits(typeof(RazorEngineTemplateBase));

            builderAction?.Invoke(compilationOptionsBuilder);
            if (addPdb)
            {
                MemoryStream pdbStream = new MemoryStream();
                MemoryStream memoryStream = this.CreateAndCompileToStream(content, compilationOptionsBuilder.Options, pdbStream);
                return new RazorEngineCompiledTemplate(memoryStream,pdbStream);
            }
            else
            {
                MemoryStream memoryStream = this.CreateAndCompileToStream(content, compilationOptionsBuilder.Options);
                return new RazorEngineCompiledTemplate(memoryStream);
            }

        }

        public Task<IRazorEngineCompiledTemplate> CompileAsync(string content, Action<IRazorEngineCompilationOptionsBuilder> builderAction = null)
        {
            return Task.Factory.StartNew(() => this.Compile(content: content, builderAction: builderAction));
        }

        private MemoryStream CreateAndCompileToStream(string templateSource, RazorEngineCompilationOptions options, MemoryStream pdbStream = null)
        {
            templateSource = this.WriteDirectives(templateSource, options);
            string projectPath = @".";
            string fileName = Path.GetRandomFileName()+".cshtml";
            if (pdbStream != null)
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

            MemoryStream memoryStream = new MemoryStream();
            EmitResult emitResult;
            if (pdbStream != null)
                emitResult = compilation.Emit(memoryStream, pdbStream);
            else
                emitResult = compilation.Emit(memoryStream);

            if (!emitResult.Success)
            {
                RazorEngineCompilationException exception = new RazorEngineCompilationException()
                {
                    Errors = emitResult.Diagnostics.ToList(),
                    GeneratedCode = razorCSharpDocument.GeneratedCode
                };

                throw exception;
            }

            memoryStream.Position = 0;
            if (pdbStream != null) 
                pdbStream.Position = 0;
            return memoryStream;
        }

        private string WriteDirectives(string content, RazorEngineCompilationOptions options)
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