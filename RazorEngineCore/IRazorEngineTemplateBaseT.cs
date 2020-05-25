namespace RazorEngineCore
{
    public interface IRazorEngineTemplateBase<T> :IRazorEngineTemplateBase
    {
        new T Model { get; set; }
    }
}