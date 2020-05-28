namespace RazorEngineCore
{
    public interface IRazorEngineTemplateBase<T> : IRazorEngineTemplate
    {
        new T Model { get; set; }
    }
}