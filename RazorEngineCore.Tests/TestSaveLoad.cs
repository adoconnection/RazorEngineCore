using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RazorEngineCore.Tests.Models;
using RazorEngineCore.Tests.Templates;

namespace RazorEngineCore.Tests
{
    [TestClass]
    public class TestSaveLoad
    {
        [TestMethod]
        public void TestSaveToStream()
        {
            RazorEngine razorEngine = new RazorEngine();
            RazorEngineCompiledTemplate initialTemplate = razorEngine.Compile("Hello @Model.Name");
            
            MemoryStream memoryStream = new MemoryStream();
            initialTemplate.SaveToStream(memoryStream);
            memoryStream.Position = 0;

            RazorEngineCompiledTemplate loadedTemplate = RazorEngineCompiledTemplate.LoadFromStream(memoryStream);

            string initialTemplateResult = initialTemplate.Run(new { Name = "Alex" });
            string loadedTemplateResult = loadedTemplate.Run(new { Name = "Alex" });
            
            Assert.AreEqual(initialTemplateResult, loadedTemplateResult);
        }
        
        [TestMethod]
        public async Task TestSaveToStreamAsync()
        {
            RazorEngine razorEngine = new RazorEngine();
            RazorEngineCompiledTemplate initialTemplate = await razorEngine.CompileAsync("Hello @Model.Name");
            
            MemoryStream memoryStream = new MemoryStream();
            await initialTemplate.SaveToStreamAsync(memoryStream);
            memoryStream.Position = 0;

            RazorEngineCompiledTemplate loadedTemplate = await RazorEngineCompiledTemplate.LoadFromStreamAsync(memoryStream);

            string initialTemplateResult = await initialTemplate.RunAsync(new { Name = "Alex" });
            string loadedTemplateResult = await loadedTemplate.RunAsync(new { Name = "Alex" });
            
            Assert.AreEqual(initialTemplateResult, loadedTemplateResult);
        }
        
        [TestMethod]
        public void TestSaveToFile()
        {
            RazorEngine razorEngine = new RazorEngine();
            RazorEngineCompiledTemplate initialTemplate = razorEngine.Compile("Hello @Model.Name");
            
            initialTemplate.SaveToFile($"{nameof(TestSaveToFile)}.dll");

            RazorEngineCompiledTemplate loadedTemplate = RazorEngineCompiledTemplate.LoadFromFile("testTemplate.dll");

            string initialTemplateResult = initialTemplate.Run(new { Name = "Alex" });
            string loadedTemplateResult = loadedTemplate.Run(new { Name = "Alex" });
            
            Assert.AreEqual(initialTemplateResult, loadedTemplateResult);
        }
        
        [TestMethod]
        public async Task TestSaveToFileAsync()
        {
            RazorEngine razorEngine = new RazorEngine();
            RazorEngineCompiledTemplate initialTemplate = await razorEngine.CompileAsync("Hello @Model.Name");
            
            await initialTemplate.SaveToFileAsync($"{nameof(TestSaveToFileAsync)}.dll");

            RazorEngineCompiledTemplate loadedTemplate = await RazorEngineCompiledTemplate.LoadFromFileAsync($"{nameof(TestSaveToFileAsync)}.dll");

            string initialTemplateResult = await initialTemplate.RunAsync(new { Name = "Alex" });
            string loadedTemplateResult = await loadedTemplate.RunAsync(new { Name = "Alex" });
            
            Assert.AreEqual(initialTemplateResult, loadedTemplateResult);
        }
        
        [TestMethod]
        public void TestSaveToFileWithModel()
        {
            RazorEngine razorEngine = new RazorEngine();
            RazorEngineCompiledTemplate<RazorEngineTemplateBase<TestSaveModel>> initialTemplate = razorEngine.Compile<RazorEngineTemplateBase<TestSaveModel>>("Hello @Model.Name");
            
            initialTemplate.SaveToFile($"{nameof(TestSaveToFileWithModel)}.dll");

            RazorEngineCompiledTemplate loadedTemplate = RazorEngineCompiledTemplate.LoadFromFile($"{nameof(TestSaveToFileWithModel)}.dll");

            var model = new TestSaveModel()
            {
                Name = "Alex"
            };
            
            string initialTemplateResult = initialTemplate.Run(model);
            
            string loadedTemplateResult = loadedTemplate.Run(model);
            
            Assert.AreEqual(initialTemplateResult, loadedTemplateResult);
        }
        
        [TestMethod]
        public async Task TestSaveToFileWithModelAsync()
        {
            RazorEngine razorEngine = new RazorEngine();
            RazorEngineCompiledTemplate<RazorEngineTemplateBase<TestSaveModel>> initialTemplate = await razorEngine.CompileAsync<RazorEngineTemplateBase<TestSaveModel>>("Hello @Model.Name");
            
            await initialTemplate.SaveToFileAsync($"{nameof(TestSaveToFileWithModelAsync)}.dll");

            RazorEngineCompiledTemplate loadedTemplate = await RazorEngineCompiledTemplate.LoadFromFileAsync($"{nameof(TestSaveToFileWithModelAsync)}.dll");

            var model = new TestSaveModel()
            {
                Name = "Alex"
            };
            
            string initialTemplateResult = await initialTemplate.RunAsync(model);
            string loadedTemplateResult = await loadedTemplate.RunAsync(model);
            
            Assert.AreEqual(initialTemplateResult, loadedTemplateResult);
        }
        
        
        [TestMethod]
        public async Task TestSaveToFileWithTestModel2Async()
        {
            RazorEngine razorEngine = new RazorEngine();
            RazorEngineCompiledTemplate<TestModel2> initialTemplate = await razorEngine.CompileAsync<TestModel2>("Hello @Model.C");
            
            await initialTemplate.SaveToFileAsync($"{nameof(TestSaveToFileWithTestModel2Async)}.dll");

            RazorEngineCompiledTemplate loadedTemplate = await RazorEngineCompiledTemplate.LoadFromFileAsync($"{nameof(TestSaveToFileWithTestModel2Async)}.dll");

            TestModel1 model = new TestModel1()
            {
                A = 1,
                B = 2,
                C = "Alex",
            };
            
            string initialTemplateResult = await initialTemplate.RunAsync(model);
            string loadedTemplateResult = await loadedTemplate.RunAsync(model);
            
            Assert.AreEqual(initialTemplateResult, loadedTemplateResult);
        }
        
        [TestMethod]
        public async Task TestCshtmlFileWithModelAsync()
        {
            RazorEngine razorEngine = new RazorEngine();
            
            var fileName = Path.Combine(Directory.GetCurrentDirectory(), "Content", "Test.cshtml");
            var content = await File.ReadAllTextAsync(fileName);
            
            RazorEngineCompiledTemplate<RazorEngineTemplateBase<TestModel1>> initialTemplate = await razorEngine.CompileAsync<RazorEngineTemplateBase<TestModel1>>(content);
            
            await initialTemplate.SaveToFileAsync($"{nameof(TestCshtmlFileWithModelAsync)}.dll");

            RazorEngineCompiledTemplate loadedTemplate = await RazorEngineCompiledTemplate.LoadFromFileAsync($"{nameof(TestCshtmlFileWithModelAsync)}.dll");

            var model = new TestModel1()
            {
                A = 10, 
                B = 20,
                C = $"{nameof(RazorEngineCore)}",
                D = DateTime.UtcNow,
                Numbers = new List<int>() { 2, 3, 5, 7, 11, 13}
            };
            
            string initialTemplateResult = await initialTemplate.RunAsync(model);
            string loadedTemplateResult = await loadedTemplate.RunAsync(model);
            
            Assert.AreEqual(initialTemplateResult, loadedTemplateResult);
        }
        
        [TestMethod]
        public async Task TestCshtmlFileWithCustomTemplateAsync()
        {
            RazorEngine razorEngine = new RazorEngine();

            var fileName = Path.Combine(Directory.GetCurrentDirectory(), "Content", "TestCustomTemplate.cshtml");
            var content = await File.ReadAllTextAsync(fileName);
            
            RazorEngineCompiledTemplate<CustomPageTemplate> initialTemplate = await razorEngine.CompileAsync<CustomPageTemplate>(content);
            
            await initialTemplate.SaveToFileAsync($"{nameof(TestCshtmlFileWithCustomTemplateAsync)}.dll");

            RazorEngineCompiledTemplate loadedTemplate = await RazorEngineCompiledTemplate.LoadFromFileAsync($"{nameof(TestCshtmlFileWithCustomTemplateAsync)}.dll");

            var model = new TestModel1()
            {
                A = 10, 
                B = 20,
                C = $"{nameof(RazorEngineCore)}",
                D = DateTime.UtcNow,
                Numbers = new List<int>() { 2, 3, 5, 7, 11, 13}
            };
            
            string initialTemplateResult = await initialTemplate.RunAsync(model);
            
            string loadedTemplateResult = await loadedTemplate.RunAsync(model);
            
            Assert.AreEqual(initialTemplateResult, loadedTemplateResult);
        }
    }
}