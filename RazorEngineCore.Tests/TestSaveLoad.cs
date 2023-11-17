using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RazorEngineCore.Tests.Models;

namespace RazorEngineCore.Tests
{
    [TestClass]
    public class TestSaveLoad
    {
        [TestMethod]
        public void TestSaveToStream()
        {
            RazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate initialTemplate = razorEngine.Compile("Hello @Model.Name");
            
            MemoryStream memoryStream = new MemoryStream();
            initialTemplate.SaveToStream(memoryStream);
            memoryStream.Position = 0;

            IRazorEngineCompiledTemplate loadedTemplate = RazorEngineCompiledTemplate.LoadFromStream(memoryStream);

            string initialTemplateResult = initialTemplate.Run(new { Name = "Alex" });
            string loadedTemplateResult = loadedTemplate.Run(new { Name = "Alex" });
            
            Assert.AreEqual(initialTemplateResult, loadedTemplateResult);
        }

        [TestMethod]
        public async Task TestSaveToStreamAsync()
        {
            RazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate initialTemplate = await razorEngine.CompileAsync("Hello @Model.Name");
            
            MemoryStream memoryStream = new MemoryStream();
            await initialTemplate.SaveToStreamAsync(memoryStream);
            memoryStream.Position = 0;

            IRazorEngineCompiledTemplate loadedTemplate = await RazorEngineCompiledTemplate.LoadFromStreamAsync(memoryStream);

            string initialTemplateResult = await initialTemplate.RunAsync(new { Name = "Alex" });
            string loadedTemplateResult = await loadedTemplate.RunAsync(new { Name = "Alex" });
            
            Assert.AreEqual(initialTemplateResult, loadedTemplateResult);
        }
        
        [TestMethod]
        public void TestSaveToFile_Typed()
        {
            RazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate<RazorEngineTemplateBase<TestModel>> initialTemplate = razorEngine.Compile<RazorEngineTemplateBase<TestModel>>("Hello @Model.A @Model.C");
            
            initialTemplate.SaveToFile("testTemplate.dll");

            IRazorEngineCompiledTemplate<RazorEngineTemplateBase<TestModel>> loadedTemplate = RazorEngineCompiledTemplate<RazorEngineTemplateBase<TestModel>>.LoadFromFile("testTemplate.dll");

            Action<RazorEngineTemplateBase<TestModel>> action = initializer =>
            {
                initializer.Model = new TestModel()
                {
                    A = 12345,
                    C = "Alex"
                };
            };

            string initialTemplateResult = initialTemplate.Run(action);
            string loadedTemplateResult = loadedTemplate.Run(action);
            
            Assert.AreEqual(initialTemplateResult, loadedTemplateResult);
        }
        
        [TestMethod]
        public void TestSaveToFile_Anonymous()
        {
            RazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate initialTemplate = razorEngine.Compile("Hello @Model.Name");
            
            initialTemplate.SaveToFile("testTemplate.dll");

            IRazorEngineCompiledTemplate loadedTemplate = RazorEngineCompiledTemplate.LoadFromFile("testTemplate.dll");

            string initialTemplateResult = initialTemplate.Run(new { Name = "Alex" });
            string loadedTemplateResult = loadedTemplate.Run(new { Name = "Alex" });
            
            Assert.AreEqual(initialTemplateResult, loadedTemplateResult);
        }
        
        [TestMethod]
        public async Task TestSaveToFileAsync()
        {
            RazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate initialTemplate = await razorEngine.CompileAsync("Hello @Model.Name");
            
            await initialTemplate.SaveToFileAsync("testTemplate.dll");

            IRazorEngineCompiledTemplate loadedTemplate = await RazorEngineCompiledTemplate.LoadFromFileAsync("testTemplate.dll");

            string initialTemplateResult = await initialTemplate.RunAsync(new { Name = "Alex" });
            string loadedTemplateResult = await loadedTemplate.RunAsync(new { Name = "Alex" });
            
            Assert.AreEqual(initialTemplateResult, loadedTemplateResult);
        }
        
        [TestMethod]
        public async Task TestSave_RazorEngineCompiledTemplateMeta_1()
        {
            RazorEngineCompiledTemplateMeta meta1 = new RazorEngineCompiledTemplateMeta()
            {
                AssemblyByteCode = new byte[] { 1, 2, 3 },
                TemplateFileName = "name1",
                TemplateNamespace = "namespace1"
            };

            MemoryStream memoryStream = new MemoryStream();

            await meta1.Write(memoryStream);
            memoryStream.Position = 0;

            RazorEngineCompiledTemplateMeta meta2 = await RazorEngineCompiledTemplateMeta.Read(memoryStream);

            CollectionAssert.AreEqual(meta1.AssemblyByteCode, meta2.AssemblyByteCode);
            CollectionAssert.AreEqual(meta1.PdbByteCode, meta2.PdbByteCode);
            Assert.AreEqual(meta1.TemplateFileName, meta2.TemplateFileName);
            Assert.AreEqual(meta1.TemplateNamespace, meta2.TemplateNamespace);
            Assert.AreEqual(meta1.GeneratedSourceCode, meta2.GeneratedSourceCode);
            Assert.AreEqual(meta1.TemplateSource, meta2.TemplateSource);
        }
        
        [TestMethod]
        public async Task TestSave_RazorEngineCompiledTemplateMeta_2()
        {
            RazorEngineCompiledTemplateMeta meta1 = new RazorEngineCompiledTemplateMeta()
            {
                AssemblyByteCode = new byte[] { 1, 2, 3 },
                PdbByteCode = new byte[] { 1, 2, 3 },
                TemplateFileName = "111",
                TemplateNamespace = "222",
                GeneratedSourceCode = "33333",
                TemplateSource = "44444"

            };

            MemoryStream memoryStream = new MemoryStream();

            await meta1.Write(memoryStream);
            memoryStream.Position = 0;

            RazorEngineCompiledTemplateMeta meta2 = await RazorEngineCompiledTemplateMeta.Read(memoryStream);


            CollectionAssert.AreEqual(meta1.AssemblyByteCode, meta2.AssemblyByteCode);
            CollectionAssert.AreEqual(meta1.PdbByteCode, meta2.PdbByteCode);
            Assert.AreEqual(meta1.TemplateFileName, meta2.TemplateFileName);
            Assert.AreEqual(meta1.TemplateNamespace, meta2.TemplateNamespace);
            Assert.AreEqual(meta1.GeneratedSourceCode, meta2.GeneratedSourceCode);
            Assert.AreEqual(meta1.TemplateSource, meta2.TemplateSource);
        }
    }
}