namespace RazorEngineCore.Tests.Models
{
    public class TestTemplate2 : RazorEngineTemplateBase<TestModel>
    {
        public void Initialize(TestModel model)
        {
            this.Model = model;
        }
    }
}