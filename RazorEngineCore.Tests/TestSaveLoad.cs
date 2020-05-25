using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
            
            initialTemplate.SaveToFile("testTemplate.dll");

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
            
            await initialTemplate.SaveToFileAsync("testTemplate.dll");

            RazorEngineCompiledTemplate loadedTemplate = await RazorEngineCompiledTemplate.LoadFromFileAsync("testTemplate.dll");

            string initialTemplateResult = await initialTemplate.RunAsync(new { Name = "Alex" });
            string loadedTemplateResult = await loadedTemplate.RunAsync(new { Name = "Alex" });
            
            Assert.AreEqual(initialTemplateResult, loadedTemplateResult);
        }
        
        [TestMethod]
        public void TestSaveToFileWithModel()
        {
            RazorEngine razorEngine = new RazorEngine();
            RazorEngineCompiledTemplate<TestSaveModel> initialTemplate = razorEngine.Compile<TestSaveModel>("Hello @Model.Name");
            
            initialTemplate.SaveToFile("testTemplate.dll");

            RazorEngineCompiledTemplate loadedTemplate = RazorEngineCompiledTemplate.LoadFromFile("testTemplate.dll");

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
            RazorEngineCompiledTemplate initialTemplate = await razorEngine.CompileAsync<TestSaveModel>("Hello @Model.Name");
            
            await initialTemplate.SaveToFileAsync("testTemplate.dll");

            RazorEngineCompiledTemplate loadedTemplate = await RazorEngineCompiledTemplate.LoadFromFileAsync("testTemplate.dll");

            var model = new TestSaveModel()
            {
                Name = "Alex"
            };
            
            string initialTemplateResult = await initialTemplate.RunAsync(model);
            string loadedTemplateResult = await loadedTemplate.RunAsync(model);
            
            Assert.AreEqual(initialTemplateResult, loadedTemplateResult);
        }
        
        [TestMethod]
        public async Task TestCshtmlFileWithModelAsync()
        {
            RazorEngine razorEngine = new RazorEngine();
            
            var assembly = GetType().GetTypeInfo().Assembly;
            var fileName = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "Test.cshtml");
            var content = await File.ReadAllTextAsync(fileName);
            
            RazorEngineCompiledTemplate initialTemplate = await razorEngine.CompileAsync<TestModel1>(content);
            
            await initialTemplate.SaveToFileAsync("testTemplate.dll");

            RazorEngineCompiledTemplate loadedTemplate = await RazorEngineCompiledTemplate.LoadFromFileAsync("testTemplate.dll");

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
            
            var assembly = GetType().GetTypeInfo().Assembly;
            var fileName = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "TestCustomTemplate.cshtml");
            var content = await File.ReadAllTextAsync(fileName);
            
            RazorEngineCompiledTemplate initialTemplate = await razorEngine.CompileAsync<CustomPageModel>(content);
            
            await initialTemplate.SaveToFileAsync("testTemplate.dll");

            RazorEngineCompiledTemplate loadedTemplate = await RazorEngineCompiledTemplate.LoadFromFileAsync("testTemplate.dll");

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