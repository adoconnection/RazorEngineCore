using RazorEngineCore.Tests.Models;

namespace RazorEngineCore.Tests.Templates
{
    public abstract class CustomPageTemplate : RazorEngineTemplateBase<TestModel1>
    {
        public dynamic AxB()
        {
            return $"{nameof(Model.A)} x {nameof(Model.B)} = {Model.A * Model.B}";
        }
        
    }
}