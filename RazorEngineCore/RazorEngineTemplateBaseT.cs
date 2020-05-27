﻿namespace RazorEngineCore
{
    public abstract class RazorEngineTemplateBase<T> : RazorEngineTemplateBase, IRazorEngineTemplateBase, IRazorEngineTemplateBase<T>
    {
        public new T Model { get; set; }
    }
}   