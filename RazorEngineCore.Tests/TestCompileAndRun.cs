using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RazorEngineCore.Tests.Models;

namespace RazorEngineCore.Tests
{
    using System.Runtime.InteropServices;

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
            IRazorEngineCompiledTemplate<string> template = razorEngine.Compile("<h1>Hello @Model.Name</h1>");

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
            IRazorEngineCompiledTemplate<string> template = await razorEngine.CompileAsync("<h1>Hello @Model.Name</h1>");

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
            IRazorEngineCompiledTemplate<string> template = razorEngine.Compile("<div class=\"circle\" style=\"background-color: hsla(@Model.Colour, 70%,   80%,1);\">");

            string actual = template.Run(new
            {
                Colour = 88
            });

            Assert.AreEqual("<div class=\"circle\" style=\"background-color: hsla(88, 70%,   80%,1);\">", actual);
        }

        [TestMethod]
        public void TestCompileAndRun_InAttributeVariables2()
        {
            RazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate<string> template = razorEngine.Compile("<img src='@(\"test\")'>");

            string actual = template.Run(new
            {
                Colour = 88
            });

            Assert.AreEqual("<img src='test'>", actual);
        }

        [TestMethod]
        public async Task TestCompileAndRun_InAttributeVariablesAsync()
        {
            RazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate<string> template = await razorEngine.CompileAsync("<div class=\"circle\" style=\"background-color: hsla(@Model.Colour, 70%,   80%,1);\">");

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
            IRazorEngineCompiledTemplate<string> template = razorEngine.Compile("<div title=\"@Model.Name\">Hello</div>");

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
            IRazorEngineCompiledTemplate<string> template = await razorEngine.CompileAsync("<div title=\"@Model.Name\">Hello</div>");

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
            IRazorEngineCompiledTemplate<string> template = razorEngine.Compile("Hello @Model.Name");

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
            IRazorEngineCompiledTemplate<string> template = await razorEngine.CompileAsync("Hello @Model.Name");

            string actual = await template.RunAsync(new
            {
                Name = "Alex"
            });

            Assert.AreEqual("Hello Alex", actual);
        }

        public struct Item
        {
            public string Name { get; set; }
        }

        [TestMethod]
        public void TestCompileAndRun_StructList()
        {
            var eng = new RazorEngine();
            var model = new
            {
                Items = new[] { new Item { Name = "Bob" }, new Item { Name = "Alice" } }
            };
            var temp = eng.Compile("@foreach(var item in Model.Items) { @item.Name }");
            var result = temp.Run(model);
            Assert.AreEqual("BobAlice", result);
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
        public void TestCompileAndRun_NullablePropertyWithValue()
        {
            RazorEngine razorEngine = new RazorEngine();

            DateTime? dateTime = DateTime.Now;

            IRazorEngineCompiledTemplate<TestTemplate2, string> template = razorEngine.Compile<TestTemplate2>("DateTime: @Model.DateTime.Value.ToString()");

            string actual = template.Run(instance => instance.Model = new TestModel()
            {
                    DateTime = dateTime
            });

            Assert.AreEqual("DateTime: " + dateTime, actual);
        }

        [TestMethod]
        public void TestCompileAndRun_NullablePropertyWithoutValue()
        {
            RazorEngine razorEngine = new RazorEngine();

            DateTime? dateTime = null;

            IRazorEngineCompiledTemplate<TestTemplate2, string> template = razorEngine.Compile<TestTemplate2>("DateTime: @Model.DateTime");

            string actual = template.Run(instance => instance.Model = new TestModel()
            {
                    DateTime = dateTime
            });

            Assert.AreEqual("DateTime: " + dateTime, actual);
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
        public void TestCompileAndRun_DynamicModel_Dictionary1()
        {
            RazorEngine razorEngine = new RazorEngine();

            var model = new
            {
                Dictionary = new Dictionary<string, object>()
                {
                    { "K1", "V1"},
                    { "K2", "V2"},
                }
            };

            var template = razorEngine.Compile(@"
@foreach (var key in Model.Dictionary.Keys)
{
<div>@key</div>
}
<div>@Model.Dictionary[""K1""]</div>
<div>@Model.Dictionary[""K2""]</div>
");

            string actual = template.Run(model);
            string expected = @"
<div>K1</div>
<div>K2</div>
<div>V1</div>
<div>V2</div>
";
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void TestCompileAndRun_DynamicModel_Dictionary2()
        {
            RazorEngine razorEngine = new RazorEngine();

            var model = new
            {
                Dictionary = new Dictionary<string, object>()
                {
                    { "K1", new { x = 1 } },
                    { "K2", new { x = 2 } },
                }
            };

            var template = razorEngine.Compile(@"
<div>@Model.Dictionary[""K1""].x</div>
<div>@Model.Dictionary[""K2""].x</div>
");

            string actual = template.Run(model);
            string expected = @"
<div>1</div>
<div>2</div>
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
            IRazorEngineCompiledTemplate<TestTemplate1, string> template = razorEngine.Compile<TestTemplate1>("Hello @A @B @(A + B) @C @Decorator(\"777\")");

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
            IRazorEngineCompiledTemplate<TestTemplate1, string> template = await razorEngine.CompileAsync<TestTemplate1>("Hello @A @B @(A + B) @C @Decorator(\"777\")");

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
            IRazorEngineCompiledTemplate<TestTemplate2, string> template = razorEngine.Compile<TestTemplate2>("Hello @Model.Decorator(Model.C)");

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
        public void TestCompileAndRun_TypedModel3()
        {
            string templateText = @"
@inherits RazorEngineCore.RazorEngineTemplateBase<RazorEngineCore.Tests.Models.TestModel>
Hello @Model.Decorator(Model.C)
";

            RazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate<RazorEngineTemplateBase<TestModel>, string> template = razorEngine.Compile<RazorEngineTemplateBase<TestModel>>(templateText);

            string actual = template.Run(instance =>
            {
                instance.Model = new TestModel
                {
                        C = "Alex"
                };
            });

            Assert.AreEqual("Hello -=Alex=-", actual.Trim());
        }

        [TestMethod]
        public async Task TestCompileAndRun_TypedModel2Async()
        {
            RazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate<TestTemplate2, string> template = await razorEngine.CompileAsync<TestTemplate2>("Hello @Model.Decorator(Model.C)");

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
        public void TestCompileAndRun_AnonymousModelWithArrayOfObjects()
        {
            RazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate<TestTemplate2, string> template = razorEngine.Compile<TestTemplate2>(
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
                    Numbers = new[] { 2, 1, 3 }
                });
            });

            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void TestCompileAndRun_StronglyTypedModelLinq()
        {
            RazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate<TestTemplate2, string> template = razorEngine.Compile<TestTemplate2>(
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
                    Numbers = new[] { 2, 1, 3 }
                });
            });

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCompileAndRun_DynamicModelLinq()
        {
            RazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate<string> template = razorEngine.Compile(
@"
@foreach (var item in ((IEnumerable<object>)Model.Numbers).OrderByDescending(x => x))
{
    <p>@item</p>
}
");
            string expected = @"
    <p>3</p>
    <p>2</p>
    <p>1</p>
";
            string actual = template.Run(new
            {
                    Numbers = new List<object>() {2, 1, 3}
            });

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async Task TestCompileAndRun_LinqAsync()
        {
            RazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate<TestTemplate2, string> template = await razorEngine.CompileAsync<TestTemplate2>(
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
                    Numbers = new[] { 2, 1, 3 }
                });
            });

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async Task TestCompileAndRun_MetadataReference()
        {
            string greetingClass = @"
namespace TestAssembly
{
    public static class Greeting
    {
        public static string GetGreeting(string name)
        {
            return ""Hello, "" + name + ""!"";
        }
    }
}
";
            // This needs to be done in the builder to have access to all of the assemblies added through
            // the various AddAssemblyReference options
            CSharpCompilation compilation = CSharpCompilation.Create(
                    "TestAssembly",
                    new[]
                    {
                            CSharpSyntaxTree.ParseText(greetingClass)
                    },
                    GetMetadataReferences(),
                    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            );

            MemoryStream memoryStream = new MemoryStream();
            EmitResult emitResult = compilation.Emit(memoryStream);

            if (!emitResult.Success)
            {
                Assert.Fail("Unable to compile test assembly");
            }

            memoryStream.Position = 0;

            // Add an assembly resolver so the assembly can be found
            AppDomain.CurrentDomain.AssemblyResolve += (sender, eventArgs) =>
                    new AssemblyName(eventArgs.Name ?? string.Empty).Name == "TestAssembly"
                            ? Assembly.Load(memoryStream.ToArray())
                            : null;

            RazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate<string> template = await razorEngine.CompileAsync(@"
@using TestAssembly
<p>@Greeting.GetGreeting(""Name"")</p>
", builder =>
            {
                builder.AddMetadataReference(MetadataReference.CreateFromStream(memoryStream));

            });

            string expected = @"
<p>Hello, Name!</p>
";
            string actual = await template.RunAsync();

            Assert.AreEqual(expected, actual);
        }

        private static List<MetadataReference> GetMetadataReferences()
        {
            if (RuntimeInformation.FrameworkDescription.StartsWith(
                ".NET Framework",
                StringComparison.OrdinalIgnoreCase))
            {
                return new List<MetadataReference>()
                           {
                               MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                               MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.CSharp, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")).Location),
                               MetadataReference.CreateFromFile(Assembly.Load(
                                   new AssemblyName(
                                       "netstandard, Version=2.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51")).Location),
                               MetadataReference.CreateFromFile(typeof(System.Runtime.GCSettings).Assembly.Location)
                           };
            }

            return new List<MetadataReference>()
                       {
                           MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                           MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.CSharp")).Location),
                           MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("netstandard")).Location),
                           MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Runtime")).Location)
                       };
        }
    }
}
