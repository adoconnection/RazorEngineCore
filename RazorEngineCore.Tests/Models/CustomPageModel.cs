namespace RazorEngineCore.Tests.Models
{
    public class CustomPageModel : RazorEngineTemplateBase<TestModel1>
    {
        public dynamic AxB()
        {
            return $"{nameof(Model.A)} x {nameof(Model.B)} = {Model.A * Model.B}";
        }
    }
}