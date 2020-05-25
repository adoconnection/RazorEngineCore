﻿namespace RazorEngineCore
{
    public abstract class RazorEngineTemplateBase<T> : RazorEngineTemplateBase, IRazorEngineTemplateBase<T>
    {
        public new T Model { get; set; }
    }
}   