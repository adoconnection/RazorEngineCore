using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RazorEngineCore.Tests
{
    [TestClass]
    public class TestSaveLoad
    {
        [TestMethod]
        public void TestSaveToStream()
        {
            RazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate<string> initialTemplate = razorEngine.Compile("Hello @Model.Name");
            
            MemoryStream memoryStream = new MemoryStream();
            initialTemplate.SaveToStream(memoryStream);
            memoryStream.Position = 0;

            IRazorEngineCompiledTemplate<string> loadedTemplate = RazorEngineCompiledTemplate<string>.LoadFromStream(memoryStream);

            string initialTemplateResult = initialTemplate.Run(new { Name = "Alex" });
            string loadedTemplateResult = loadedTemplate.Run(new { Name = "Alex" });
            
            Assert.AreEqual(initialTemplateResult, loadedTemplateResult);
        }
        
        [TestMethod]
        public async Task TestSaveToStreamAsync()
        {
            RazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate<string> initialTemplate = await razorEngine.CompileAsync("Hello @Model.Name");
            
            MemoryStream memoryStream = new MemoryStream();
            await initialTemplate.SaveToStreamAsync(memoryStream);
            memoryStream.Position = 0;

            IRazorEngineCompiledTemplate<string> loadedTemplate = await RazorEngineCompiledTemplate<string>.LoadFromStreamAsync(memoryStream);

            string initialTemplateResult = await initialTemplate.RunAsync(new { Name = "Alex" });
            string loadedTemplateResult = await loadedTemplate.RunAsync(new { Name = "Alex" });
            
            Assert.AreEqual(initialTemplateResult, loadedTemplateResult);
        }
        
        [TestMethod]
        public void TestSaveToFile()
        {
            RazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate<string> initialTemplate = razorEngine.Compile("Hello @Model.Name");
            
            initialTemplate.SaveToFile("testTemplate.dll");

            IRazorEngineCompiledTemplate<string> loadedTemplate = RazorEngineCompiledTemplate<string>.LoadFromFile("testTemplate.dll");

            string initialTemplateResult = initialTemplate.Run(new { Name = "Alex" });
            string loadedTemplateResult = loadedTemplate.Run(new { Name = "Alex" });
            
            Assert.AreEqual(initialTemplateResult, loadedTemplateResult);
        }
        
        [TestMethod]
        public async Task TestSaveToFileAsync()
        {
            RazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate<string> initialTemplate = await razorEngine.CompileAsync("Hello @Model.Name");
            
            await initialTemplate.SaveToFileAsync("testTemplate.dll");

            IRazorEngineCompiledTemplate<string> loadedTemplate = await RazorEngineCompiledTemplate<string>.LoadFromFileAsync("testTemplate.dll");

            string initialTemplateResult = await initialTemplate.RunAsync(new { Name = "Alex" });
            string loadedTemplateResult = await loadedTemplate.RunAsync(new { Name = "Alex" });
            
            Assert.AreEqual(initialTemplateResult, loadedTemplateResult);
        }
    }
}