using System;
using System.Collections.Generic;

namespace RazorEngineCore.Tests.Models
{
    public class TestModel2 : RazorEngineTemplateBase//<TestModel1>
    {
        public new TestModel1 Model { get; set; }

        public void Initialize(TestModel1 model)
        {
            this.Model = model;
        } 
    }
}