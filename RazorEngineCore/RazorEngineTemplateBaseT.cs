﻿namespace RazorEngineCore
{
    public abstract class RazorEngineTemplateBase<TTemplate> : RazorEngineTemplateBase, IRazorEngineTemplate<TTemplate>
    {
        public new TTemplate Model { get; set; }
    }
}   