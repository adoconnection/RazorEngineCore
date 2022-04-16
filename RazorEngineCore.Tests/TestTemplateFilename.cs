using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RazorEngineCore.Tests
{
    [TestClass]
    public class TestTemplateFilename
    {
        [TestMethod]
        public void TestSettingTemplateFilename()
        {
            RazorEngine razorEngine = new RazorEngine();
            var errorThrown = false;
            try
            {
                IRazorEngineCompiledTemplate<string> initialTemplate = razorEngine.Compile("@{ this is a syntaxerror }", 
                    builder => { builder.Options.TemplateFilename = "templatefilenameset.txt"; });
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("templatefilenameset.txt"));
                errorThrown = true;
            }

            Assert.IsTrue(errorThrown);
        }
    }
}
