using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RazorEngineCore.Tests 
{
    [TestClass]
    public class TestCompileAndRunWithIncludeAndLayout 
    {
        private class TestMethodModel : BaseModel 
        {
            public int Add (int num) => A + num;
            public int Add (string num) {
                if (!int.TryParse (num, out var result)) {
                    throw new ArgumentNullException (nameof (num));
                }
                return Add (result);
            }

            public int Add (int num1, string num2) => Add (num1) + Add (num2);

            public int Add (string num1, params int[] nums) => Add(num1) + nums.Aggregate (A, (cur, next) => cur + next);
        }

        private class BaseModel 
        {
            private BaseModel (int a) {
                this.A = a;

            }
            public int A { get; set; }

            public BaseModel()
            {
            }

            public int GetA () => A;
        }

        private class TestMethodParamArrayModel
        {
            public class NestedModel
            {

            }
            public string Test (string num1, params Exception[] testmodels) => "Test";
        }

        [TestMethod]
        public void TestCompileAndRun_SingleParamaterMethod() 
        {
            var payload = @"<div>@Model.Add(1)</div>";
            RazorEngine razorEngine = new RazorEngine ();
            var compiled = razorEngine.Compile(payload, new Dictionary<string, string>());
            var actual = compiled.Run(new TestMethodModel
            {
                A = 1
            });
            Assert.AreEqual("<div>2</div>", actual);
        }

        [TestMethod]
        public void TestCompileAndRun_OverloadMethod() 
        {
            var payload = @"<div>@Model.Add(1, ""1"")</div>";
            RazorEngine razorEngine = new RazorEngine ();
            var compiled = razorEngine.Compile(payload, new Dictionary<string, string>());
            var actual = compiled.Run(new TestMethodModel
            {
                A = 1
            });
            Assert.AreEqual("<div>4</div>", actual);
        }

        [TestMethod]
        public void TestCompileAndRun_OverloadParamArrayMethod()
        {
            var payload = @"<div>@Model.Add(""1"", 1, 1)</div>";
            RazorEngine razorEngine = new RazorEngine ();
            var compiled = razorEngine.Compile(payload, new Dictionary<string, string>());
            var actual = compiled.Run(new TestMethodModel
            {
                A = 1
            });
            Assert.AreEqual("<div>5</div>", actual);
        }

        [TestMethod]
        public void TestCompileAndRun_ParamArrayLeaveUnfilled()
        {
            var payload = @"<div>@Model.Test(""1"")</div>";
            RazorEngine razorEngine = new RazorEngine ();
            var compiled = razorEngine.Compile(payload, new Dictionary<string, string>());
            var actual = compiled.Run(new TestMethodParamArrayModel());
            Assert.AreEqual(@"<div>Test</div>", actual);
        }

        [TestMethod]
        public void TestCompileAndRun_ParamArrayStandaloneArgNumNotMatched()
        {
            var payload = @"<div>@Model.Test()</div>";
            RazorEngine razorEngine = new RazorEngine ();
            var compiled = razorEngine.Compile(payload, new Dictionary<string, string>());
            Assert.ThrowsException<RuntimeBinderException>(() => compiled.Run(new TestMethodParamArrayModel()));
        }

        [TestMethod]
        public void TestCompileAndRun_ParamArrayStandAloneTypeNotMatched()
        {
            var payload = @"<div>@Model.Test(1)</div>";
            RazorEngine razorEngine = new RazorEngine ();
            var compiled = razorEngine.Compile(payload, new Dictionary<string, string>());
            Assert.ThrowsException<RuntimeBinderException>(() => compiled.Run(new TestMethodParamArrayModel()));

        }

        [TestMethod]
        public void TestCompileAndRun_ParamArrayTypeNotMatched()
        {
            var payload = @"
@using System
<div>@Model.Test(""1"", new Exception(), 1)</div>";
            RazorEngine razorEngine = new RazorEngine ();
            var compiled = razorEngine.Compile(payload, new Dictionary<string, string>());
            Assert.ThrowsException<RuntimeBinderException>(() => compiled.Run(new TestMethodParamArrayModel()));
        }

        [TestMethod]
        public void TestCompileAndRun_NoMatchedMethod()
        {
            var payload = @"
@{
    var test = new {B = ""invalid""};
}
<div>@Model.Add(test)</div>
";
            RazorEngine razorEngine = new RazorEngine ();
            var compiled = razorEngine.Compile(payload, new Dictionary<string, string>());
            var actual = 
            Assert.ThrowsException<RuntimeBinderException>(() => compiled.Run(new TestMethodModel{ A = 1}));
        }

        [TestMethod]
        public void TestCompileAndRun_AnnonymousObject()
        {
            var payload = @"<div>@Model.A</div>";
            RazorEngine razorEngine = new RazorEngine ();
            var compiled = razorEngine.Compile(payload, new Dictionary<string, string>());
            var actual = compiled.Run(new {A = 1});
            Assert.AreEqual("<div>1</div>", actual);
        }

        [TestMethod]
        public void TestCompileAndRun_LayoutAndInclude()
        {
            var expected = @"
<title>1</title>
<div>1</div>
<b>1</b>
";

            var payload = @"
@{
    Layout = ""layout.cshtml"";
}
<div>@Model.A</div>
@Include(""include"", Model)
";
            var layout = @"
<title>@Model.A</title>@RenderBody()";
            var include = @"<b>@Model.GetA()</b>";
            RazorEngine razorEngine = new RazorEngine ();
            var compiled = razorEngine.Compile(payload, new Dictionary<string, string>()
            {
                {"layout.cshtml", layout},
                {"include", include}
            });
            var actual = compiled.Run(new TestMethodModel{A =  1});
            Assert.AreEqual(expected, actual);
        }
    }
}