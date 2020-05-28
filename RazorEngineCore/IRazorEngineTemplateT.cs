namespace RazorEngineCore
{
    public interface IRazorEngineTemplate<T> : IRazorEngineTemplate
    {
        new T Model { get; set; }
    }
}