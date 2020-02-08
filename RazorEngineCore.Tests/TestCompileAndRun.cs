using Microsoft.VisualStudio.TestTools.UnitTesting;
using RazorEngineCore.Tests.Models;

namespace RazorEngineCore.Tests
{
    [TestClass]
    public class TestCompileAndRun
    {
        [TestMethod]
        public void TestCompile()
        {
            RazorEngine razorEngine = new RazorEngine();
            razorEngine.Compile("Hello @Model.Name");
        }

        [TestMethod]
        public void TestCompileAndRun_DynamicModel()
        {
            RazorEngine razorEngine = new RazorEngine();
            RazorEngineCompiledTemplate template = razorEngine.Compile("Hello @Model.Name");
            string actual = template.Run(new { Name = "Alex" });
            Assert.AreEqual("Hello Alex", actual);
        }

        [TestMethod]
        public void TestCompileAndRun_TypedModel1()
        {
            RazorEngine razorEngine = new RazorEngine();
            RazorEngineCompiledTemplate<TestModel1> template = razorEngine.Compile<TestModel1>("Hello @A @B @(A + B) @C @Decorator(\"777\")");

            string actual = template.Run(instance =>
            {
                instance.A = 1;
                instance.B = 2;
                instance.C = "Alex";
            });

            Assert.AreEqual("Hello 1 2 3 Alex -=777=-", actual);
        }

        [TestMethod]
        public void TestCompileAndRun_TypedModel2()
        {
            RazorEngine razorEngine = new RazorEngine();
            RazorEngineCompiledTemplate<TestModel2> template = razorEngine.Compile<TestModel2>("Hello @Model.Decorator(Model.C)");

            string actual = template.Run(instance =>
            {
                instance.Initialize(new TestModel1()
                {
                    C = "Alex"
                });
            });

            Assert.AreEqual("Hello -=Alex=-", actual);
        }
    }
}
