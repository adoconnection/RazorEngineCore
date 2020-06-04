using System.Threading.Tasks;
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
        public Task TestCompileAsync()
        {
            RazorEngine razorEngine = new RazorEngine();
            return razorEngine.CompileAsync("Hello @Model.Name");
        }

        [TestMethod]
        public void TestCompileAndRun_HtmlLiteral()
        {
            RazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate template = razorEngine.Compile("<h1>Hello @Model.Name</h1>");

            string actual = template.Run(new
            {
                Name = "Alex"
            });

            Assert.AreEqual("<h1>Hello Alex</h1>", actual);
        }

        [TestMethod]
        public async Task TestCompileAndRun_HtmlLiteralAsync()
        {
            RazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate template = await razorEngine.CompileAsync("<h1>Hello @Model.Name</h1>");

            string actual = await template.RunAsync(new
            {
                Name = "Alex"
            });
            
            Assert.AreEqual("<h1>Hello Alex</h1>", actual);
        }

        [TestMethod]
        public void TestCompileAndRun_InAttributeVariables()
        {
            RazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate template = razorEngine.Compile("<div class=\"circle\" style=\"background-color: hsla(@Model.Colour, 70%,   80%,1);\">");

            string actual = template.Run(new
            {
                Colour = 88
            });

            Assert.AreEqual("<div class=\"circle\" style=\"background-color: hsla(88, 70%,   80%,1);\">", actual);
        }

        [TestMethod]
        public async Task TestCompileAndRun_InAttributeVariablesAsync()
        {
            RazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate template = await razorEngine.CompileAsync("<div class=\"circle\" style=\"background-color: hsla(@Model.Colour, 70%,   80%,1);\">");

            string actual = await template.RunAsync(new
            {
                Colour = 88
            });

            Assert.AreEqual("<div class=\"circle\" style=\"background-color: hsla(88, 70%,   80%,1);\">", actual);
        }

        [TestMethod]
        public void TestCompileAndRun_HtmlAttribute()
        {
            RazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate template = razorEngine.Compile("<div title=\"@Model.Name\">Hello</div>");

            string actual = template.Run(new
            {
                Name = "Alex"
            });

            Assert.AreEqual("<div title=\"Alex\">Hello</div>", actual);
        }

        [TestMethod]
        public async Task TestCompileAndRun_HtmlAttributeAsync()
        {
            RazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate template = await razorEngine.CompileAsync("<div title=\"@Model.Name\">Hello</div>");

            string actual = await template.RunAsync(new
            {
                Name = "Alex"
            });

            Assert.AreEqual("<div title=\"Alex\">Hello</div>", actual);
        }

        [TestMethod]
        public void TestCompileAndRun_DynamicModel_Plain()
        {
            RazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate template = razorEngine.Compile("Hello @Model.Name");

            string actual = template.Run(new
            {
                Name = "Alex"
            });

            Assert.AreEqual("Hello Alex", actual);
        }

        [TestMethod]
        public async Task TestCompileAndRun_DynamicModel_PlainAsync()
        {
            RazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate template = await razorEngine.CompileAsync("Hello @Model.Name");

            string actual = await template.RunAsync(new
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
        public async Task TestCompileAndRun_DynamicModel_NestedAsync()
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

            var template = await razorEngine.CompileAsync("Name: @Model.Name, Membership: @Model.Membership.Level");

            string actual = await template.RunAsync(model);

            Assert.AreEqual("Name: Alex, Membership: Gold", actual);
        }

        [TestMethod]
        public void TestCompileAndRun_NullModel()
        {
            RazorEngine razorEngine = new RazorEngine();

            var template = razorEngine.Compile("Name: @Model");

            string actual = template.Run(null);

            Assert.AreEqual("Name: ", actual);
        }

        [TestMethod]
        public async Task TestCompileAndRun_NullModelAsync()
        {
            RazorEngine razorEngine = new RazorEngine();

            var template = await razorEngine.CompileAsync("Name: @Model");

            string actual = await template.RunAsync(null);

            Assert.AreEqual("Name: ", actual);
        }

        [TestMethod]
        public void TestCompileAndRun_NullNestedObject()
        {
            RazorEngine razorEngine = new RazorEngine();

            var template = razorEngine.Compile("Name: @Model.user");

            string actual = template.Run(new
            {
                user = (object)null
            });

            Assert.AreEqual("Name: ", actual);
        }

        [TestMethod]
        public async Task TestCompileAndRun_NullNestedObjectAsync()
        {
            RazorEngine razorEngine = new RazorEngine();

            var template = await razorEngine.CompileAsync("Name: @Model.user");

            string actual = await template.RunAsync(new
            {
                user = (object)null
            });

            Assert.AreEqual("Name: ", actual);
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
        public async Task TestCompileAndRun_DynamicModel_ListsAsync()
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

            var template = await razorEngine.CompileAsync(@"
@foreach (var item in Model.Items)
{
<div>@item.Key</div>
}
");

            string actual = await template.RunAsync(model);
            string expected = @"
<div>K1</div>
<div>K2</div>
";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCompileAndRun_TestFunction()
        {
            RazorEngine razorEngine = new RazorEngine();

            var template = razorEngine.Compile(@"
@<area>
    @{ RecursionTest(3); }
</area>

@{

void RecursionTest(int level)
{
	if (level <= 0)
	{
		return;
	}
		
    <div>LEVEL: @level</div>
	@{ RecursionTest(level - 1); }
}

}");

            string actual = template.Run();
            string expected = @"
<area>
    <div>LEVEL: 3</div>
    <div>LEVEL: 2</div>
    <div>LEVEL: 1</div>
</area>
";
            Assert.AreEqual(expected.Trim(), actual.Trim());
        }

        [TestMethod]
        public void TestCompileAndRun_TypedModel1()
        {
            RazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate<TestTemplate1> template = razorEngine.Compile<TestTemplate1>("Hello @A @B @(A + B) @C @Decorator(\"777\")");

            string actual = template.Run(instance =>
            {
                instance.A = 1;
                instance.B = 2;
                instance.C = "Alex";
            });

            Assert.AreEqual("Hello 1 2 3 Alex -=777=-", actual);
        }

        [TestMethod]
        public async Task TestCompileAndRun_TypedModel1Async()
        {
            RazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate<TestTemplate1> template = await razorEngine.CompileAsync<TestTemplate1>("Hello @A @B @(A + B) @C @Decorator(\"777\")");

            string actual = await template.RunAsync(instance =>
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
            IRazorEngineCompiledTemplate<TestTemplate2> template = razorEngine.Compile<TestTemplate2>("Hello @Model.Decorator(Model.C)");

            string actual = template.Run(instance =>
            {
                instance.Initialize(new TestModel
                {
                    C = "Alex"
                });
            });

            Assert.AreEqual("Hello -=Alex=-", actual);
        }

        [TestMethod]
        public async Task TestCompileAndRun_TypedModel2Async()
        {
            RazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate<TestTemplate2> template = await razorEngine.CompileAsync<TestTemplate2>("Hello @Model.Decorator(Model.C)");

            string actual = await template.RunAsync(instance =>
            {
                instance.Initialize(new TestModel
                {
                    C = "Alex"
                });
            });
            
            Assert.AreEqual("Hello -=Alex=-", actual);
        }

        [TestMethod]
        public void TestCompileAndRun_Linq()
        {
            RazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate<TestTemplate2> template = razorEngine.Compile<TestTemplate2>(
@"
@foreach (var item in Model.Numbers.OrderByDescending(x => x))
{
    <p>@item</p>
}
");
            string expected = @"
    <p>3</p>
    <p>2</p>
    <p>1</p>
";
            string actual = template.Run(instance =>
            {
                instance.Initialize(new TestModel
                {
                    Numbers = new[] {2, 1, 3}
                });
            });

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async Task TestCompileAndRun_LinqAsync()
        {
            RazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate<TestTemplate2> template = await razorEngine.CompileAsync<TestTemplate2>(
@"
@foreach (var item in Model.Numbers.OrderByDescending(x => x))
{
    <p>@item</p>
}
");
            string expected = @"
    <p>3</p>
    <p>2</p>
    <p>1</p>
";
            string actual = await template.RunAsync(instance =>
            {
                instance.Initialize(new TestModel
                {
                    Numbers = new[] {2, 1, 3}
                });
            });

            Assert.AreEqual(expected, actual);
        }
    }
}
