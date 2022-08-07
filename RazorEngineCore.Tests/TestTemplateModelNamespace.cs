using Microsoft.VisualStudio.TestTools.UnitTesting;
using RazorEngineCore.Tests.Models;

namespace RazorEngineCore.Tests
{
    [TestClass]
    public class TestTemplateModelNamespace
    {
        [TestMethod]
        public void TestModelNestedTypes()
        {
            IRazorEngine razorEngine = new RazorEngine();
            string content = "Hello @Model.Name";

            IRazorEngineCompiledTemplate<RazorEngineTemplateBase<NestedTestModel.TestModelInnerClass1.TestModelInnerClass2>> template2 = razorEngine.Compile<RazorEngineTemplateBase<NestedTestModel.TestModelInnerClass1.TestModelInnerClass2>>(content);

            string result = template2.Run(instance =>
            {
                instance.Model = new NestedTestModel.TestModelInnerClass1.TestModelInnerClass2()
                {
                    Name = "Hello",
                };
            });

            Assert.AreEqual("Hello Hello", result);
        }

        [TestMethod]
        public void TestModelNoNamespace()
        {
            IRazorEngine razorEngine = new RazorEngine();
            string content = "Hello @Model.Name";

            IRazorEngineCompiledTemplate<RazorEngineTemplateBase<TestModelWithoutNamespace>> template2 = razorEngine.Compile<RazorEngineTemplateBase<TestModelWithoutNamespace>>(content);

            string result = template2.Run(instance =>
            {
                instance.Model = new TestModelWithoutNamespace()
                {
                    Name = "Hello",
                };
            });

            Assert.AreEqual("Hello Hello", result);
        }
    }
}