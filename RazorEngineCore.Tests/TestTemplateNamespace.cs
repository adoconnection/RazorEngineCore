using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RazorEngineCore.Tests.Models;

namespace RazorEngineCore.Tests
{
    [TestClass]
    public class TestTemplateNamespace
    {
        [TestMethod]
        public void TestSettingTemplateNamespace()
        {
            RazorEngine razorEngine = new RazorEngine();

            IRazorEngineCompiledTemplate initialTemplate = razorEngine.Compile("@{ var message = \"OK\"; }@message",
                builder => { builder.Options.TemplateNamespace = "Test.Namespace"; });

            var result = initialTemplate.Run();

            Assert.AreEqual("OK", result);
        }

        [TestMethod]
        public void TestSettingTemplateNamespaceT()
        {
            RazorEngine razorEngine = new RazorEngine();

            var initialTemplate = razorEngine.Compile<TestTemplate2>("@{ var message = \"OK\"; }@message",
                builder => { builder.Options.TemplateNamespace = "Test.Namespace"; });

            var result = initialTemplate.Run(a => { });

            Assert.AreEqual("OK", result);
        }
    }
}
