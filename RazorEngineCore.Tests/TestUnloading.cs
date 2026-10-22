#if NETCOREAPP
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RazorEngineCore.Tests.Models;

namespace RazorEngineCore.Tests
{
    [TestClass]
    public class TestUnloading
    {
        [TestMethod]
        public void TestDefaultIsNotCollectible()
        {
            RazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate<RazorEngineTemplateBase> template = razorEngine.Compile<RazorEngineTemplateBase>("Hello");

            bool isCollectible = true;
            template.Run(instance => isCollectible = instance.GetType().Assembly.IsCollectible);

            Assert.IsFalse(isCollectible);
        }

        [TestMethod]
        public void TestEnableUnloadingIsCollectible()
        {
            RazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate<RazorEngineTemplateBase> template = razorEngine.Compile<RazorEngineTemplateBase>("Hello", builder => builder.EnableUnloading());

            bool isCollectible = false;
            template.Run(instance => isCollectible = instance.GetType().Assembly.IsCollectible);

            Assert.IsTrue(isCollectible);
        }

        [TestMethod]
        public void TestEnableUnloadingRunsDynamicModel()
        {
            RazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate template = razorEngine.Compile("Hello @Model.Name", builder => builder.EnableUnloading());

            Assert.AreEqual("Hello Alex", template.Run(new { Name = "Alex" }));
        }

        [TestMethod]
        public void TestRunAfterDisposeThrows()
        {
            RazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate template = razorEngine.Compile("Hello", builder => builder.EnableUnloading());

            ((IDisposable)template).Dispose();

            Assert.ThrowsException<ObjectDisposedException>(() => template.Run());
        }

        [TestMethod]
        public void TestDisposeUnloadsTypedTemplate()
        {
            WeakReference context = this.CompileRunAndDispose("Hello @Model.C", new TestModel { C = "Alex" });

            Assert.IsTrue(WaitForUnload(context));
        }

        [TestMethod]
        public void TestDisposeUnloadsDynamicTemplate()
        {
            WeakReference context = this.CompileRunAndDispose("Hello @Model.Name", new { Name = "Alex" });

            Assert.IsTrue(WaitForUnload(context));
        }

        [TestMethod]
        public void TestDroppedTemplateUnloadsWithoutDispose()
        {
            WeakReference context = this.CompileRunAndDispose("Hello @Model.Name", new { Name = "Alex" }, dispose: false);

            Assert.IsTrue(WaitForUnload(context));
        }

        [TestMethod]
        public void TestLoadFromStreamWithUnloading()
        {
            RazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate initialTemplate = razorEngine.Compile("Hello @Model.Name");

            MemoryStream memoryStream = new MemoryStream();
            initialTemplate.SaveToStream(memoryStream);
            memoryStream.Position = 0;

            IRazorEngineCompiledTemplate loadedTemplate = RazorEngineCompiledTemplate.LoadFromStream(memoryStream, enableUnloading: true);

            Assert.AreEqual("Hello Alex", loadedTemplate.Run(new { Name = "Alex" }));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private WeakReference CompileRunAndDispose(string content, object model, bool dispose = true)
        {
            RazorEngine razorEngine = new RazorEngine();
            IRazorEngineCompiledTemplate<RazorEngineTemplateBase> template = razorEngine.Compile<RazorEngineTemplateBase>(content, builder => builder.EnableUnloading());

            WeakReference context = null;

            template.Run(instance =>
            {
                instance.Model = model.IsAnonymous() ? new AnonymousTypeWrapper(model) : model;
                context = new WeakReference(AssemblyLoadContext.GetLoadContext(instance.GetType().Assembly));
            });

            if (dispose)
            {
                ((IDisposable)template).Dispose();
            }

            return context;
        }

        private static bool WaitForUnload(WeakReference context)
        {
            for (int i = 0; i < 20 && context.IsAlive; i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            return !context.IsAlive;
        }
    }
}
#endif
