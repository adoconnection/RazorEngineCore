using System.IO;
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
    }
}