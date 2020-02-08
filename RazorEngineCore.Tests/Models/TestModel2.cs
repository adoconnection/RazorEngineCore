namespace RazorEngineCore.Tests.Models
{
    public class TestModel2 : RazorEngineTemplateBase
    {
        public new TestModel1 Model { get; set; }

        public void Initialize(TestModel1 model)
        {
            this.Model = model;
        } 
    }
}