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
        public void TestCompileAndRun_DynamicModel_Plain()
        {
            RazorEngine razorEngine = new RazorEngine();
            RazorEngineCompiledTemplate template = razorEngine.Compile("Hello @Model.Name");
            
            string actual = template.Run(new
            {
                Name = "Alex"
            });
            
            Assert.AreEqual("Hello Alex", actual);
        }

        [TestMethod]
        public void TestCompileAndRun_DynamicModel_Nested()
        {
            RazorEngine razorEngine = new RazorEngine();

            var model = new
            {
                Name = "Alex",
                Membership = new
                {
                    Level = "Gold"
                }
            };

            var template = razorEngine.Compile("Name: @Model.Name, Membership: @Model.Membership.Level");

            string actual = template.Run(model);
            
            Assert.AreEqual("Name: Alex, Membership: Gold", actual);
        }

        [TestMethod]
        public void TestCompileAndRun_DynamicModel_Lists()
        {
            RazorEngine razorEngine = new RazorEngine();

            var model = new
            {
                Items = new[]
                {
                    new
                    {
                        Key = "K1"
                    },
                    new
                    {
                        Key = "K2"
                    }
                }
            };

            var template = razorEngine.Compile(@"
@foreach (var item in Model.Items)
{
<div>@item.Key</div>
}
");

            string actual = template.Run(model);
            string expected = @"
<div>K1</div>
<div>K2</div>
";
            Assert.AreEqual(expected, actual);
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
