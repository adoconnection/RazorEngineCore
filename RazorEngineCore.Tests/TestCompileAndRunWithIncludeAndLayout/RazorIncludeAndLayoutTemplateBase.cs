using System;

namespace RazorEngineCore.Tests
{
    public class RazorIncludeAndLayoutTemplateBase : RazorEngineTemplateBase
    {
        public Func<string, object, string> IncludeCallback { get; set; }

        public Func<string> RenderBodyCallback { get; set; }
        public string Layout { get; set; }

        public string Include(string key, object model = null)
        {
            return this.IncludeCallback(key, model);
        }

        public string RenderBody()
        {
            return this.RenderBodyCallback();
        }
    }
}