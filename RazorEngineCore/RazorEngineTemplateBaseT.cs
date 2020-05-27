﻿namespace RazorEngineCore
{
    public abstract class RazorEngineTemplateBase<TTemplate> : RazorEngineTemplateBase, IRazorEngineTemplateBase<TTemplate>
    {
        public new TTemplate Model { get; set; }
    }
}   