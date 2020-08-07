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
            IRazorEngineCompilationOptionsBuilder compilationOptionsBuilder = new RazorEngineCompilationOptionsBuilder();
            compilationOptionsBuilder.Inherits(typeof(RazorEngineTemplateBase));
             
            builderAction?.Invoke(compilationOptionsBuilder);

            MemoryStream memoryStream = this.CreateAndCompileToStream(content, compilationOptionsBuilder.Options);

            return new RazorEngineCompiledTemplate(memoryStream);
        }

        public Task<IRazorEngineCompiledTemplate> CompileAsync(string content, Action<IRazorEngineCompilationOptionsBuilder> builderAction = null)
        {
            return Task.Factory.StartNew(() => this.Compile(content: content, builderAction: builderAction));
        }

        public IRazorEngineCompiledTemplateSet<T> CompileSet<T>(Dictionary<string, string> contents,
            Action<IRazorEngineCompilationOptionsBuilder> builderAction = null, List<string> csharpFiles = null)
            where T : IRazorEngineTemplate
        {
            IRazorEngineCompilationOptionsBuilder compilationOptionsBuilder = new RazorEngineCompilationOptionsBuilder();
            
            compilationOptionsBuilder.AddAssemblyReference(typeof(T).Assembly);
            compilationOptionsBuilder.Inherits(typeof(T));

            builderAction?.Invoke(compilationOptionsBuilder);

            MemoryStream memoryStream = this.CreateSetAndCompileToStream(contents, compilationOptionsBuilder.Options, csharpFiles);
           
            return new RazorEngineCompiledTemplateSet<T>(memoryStream, compilationOptionsBuilder.Options.TemplateNamespace);
        }

        public Task<IRazorEngineCompiledTemplateSet<T>> CompileSetAsync<T>(Dictionary<string, string> contents,
            Action<IRazorEngineCompilationOptionsBuilder> builderAction = null, List<string> csharpFiles = null)
            where T : IRazorEngineTemplate
        {
            return Task.Factory.StartNew(() => this.CompileSet<T>(contents, builderAction, csharpFiles));
        }

        public IRazorEngineCompiledTemplateSet CompileSet(Dictionary<string, string> contents,
            Action<IRazorEngineCompilationOptionsBuilder> builderAction = null, List<string> csharpFiles = null)
        {
            IRazorEngineCompilationOptionsBuilder compilationOptionsBuilder = new RazorEngineCompilationOptionsBuilder();
            compilationOptionsBuilder.Inherits(typeof(RazorEngineTemplateBase));
             
            builderAction?.Invoke(compilationOptionsBuilder);

            MemoryStream memoryStream = this.CreateSetAndCompileToStream(contents, compilationOptionsBuilder.Options, csharpFiles);

            return new RazorEngineCompiledTemplateSet(memoryStream, compilationOptionsBuilder.Options.TemplateNamespace);
        }

        public Task<IRazorEngineCompiledTemplateSet> CompileSetAsync(Dictionary<string, string> contents,
            Action<IRazorEngineCompilationOptionsBuilder> builderAction = null, List<string> csharpFiles = null)
        {
            return Task.Factory.StartNew(() => this.CompileSet(contents, builderAction, csharpFiles));
        }

        private static RazorProjectEngine GetRazorProjectEngine(string templateNamespace)
        {
            return RazorProjectEngine.Create(
                RazorConfiguration.Default,
                RazorProjectFileSystem.Create(@"."),
                (builder) =>
                {
                    builder.SetNamespace(templateNamespace);
                });
        }

        private static MemoryStream CSharpCompilationToMemoryStream(CSharpCompilation compilation, string generatedCode)
        {
            MemoryStream memoryStream = new MemoryStream();

            EmitResult emitResult = compilation.Emit(memoryStream);

            if (!emitResult.Success)
            {
                List<Diagnostic> errors = emitResult.Diagnostics.ToList();

                RazorEngineCompilationException exception = new RazorEngineCompilationException($"Unable to compile template: {errors.FirstOrDefault()}")
                {
                    Errors = errors,
                    GeneratedCode = razorCSharpDocument.GeneratedCode
                };

                throw exception;
            }

            memoryStream.Position = 0;

            return memoryStream;
        }
        
        private MemoryStream CreateAndCompileToStream(string templateSource, RazorEngineCompilationOptions options)
        {
            templateSource = this.WriteDirectives(templateSource, options);

            RazorProjectEngine engine = GetRazorProjectEngine(options.TemplateNamespace);

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
                    .Concat(options.MetadataReferences)
                    .ToList(),
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            return CSharpCompilationToMemoryStream(compilation, razorCSharpDocument.GeneratedCode);
        }

        private MemoryStream CreateSetAndCompileToStream(Dictionary<string, string> templateSources, RazorEngineCompilationOptions options, List<string> csharpFiles)
        {
            templateSources = templateSources
                .Select(templateSource =>
                    new KeyValuePair<string, string>(templateSource.Key,
                        WriteDirectives(templateSource.Value, options)))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            
            Dictionary<string, string> fileNames = templateSources
                .Select((templateSource, index) =>
                    new KeyValuePair<string, string>(templateSource.Key + index + ".cs", templateSource.Key))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            RazorProjectEngine engine = RazorProjectEngine.Create(
                RazorConfiguration.Default,
                RazorProjectFileSystem.Create(@"."),
                builder =>
                {
                    builder.SetNamespace(options.TemplateNamespace);
                    builder.ConfigureClass((document, node) =>
                    {
                        node.ClassName = fileNames[document.Source.FilePath];
                    });
                });
            
            List<SyntaxTree> syntaxTrees = templateSources.Select(templateSource =>
            {
                RazorSourceDocument document = RazorSourceDocument.Create(templateSource.Value,
                    fileNames.First(pair => pair.Value == templateSource.Key).Key);
                
                RazorCodeDocument codeDocument = engine.Process(
                    document,
                    null,
                    new List<RazorSourceDocument>(),
                    new List<TagHelperDescriptor>());

                RazorCSharpDocument razorCSharpDocument = codeDocument.GetCSharpDocument();

                return CSharpSyntaxTree.ParseText(razorCSharpDocument.GeneratedCode);
            }).ToList();
            
            CSharpCompilation compilation = CSharpCompilation.Create(
                Path.GetRandomFileName(),
                csharpFiles is null ? syntaxTrees : syntaxTrees.Concat(csharpFiles.Select(file => CSharpSyntaxTree.ParseText(file))),
                options.ReferencedAssemblies
                    .Select(ass => MetadataReference.CreateFromFile(ass.Location))
                    .Concat(options.MetadataReferences)
                    .ToList(),
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                );

            return CSharpCompilationToMemoryStream(compilation, string.Empty);
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