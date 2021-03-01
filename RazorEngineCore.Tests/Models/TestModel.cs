using System;
using System.Collections.Generic;

namespace RazorEngineCore.Tests.Models
{
    public class TestModel
    {
        public int A { get; set; }
        public int B { get; set; }
        public string C { get; set; }
        public DateTime D { get; set; }
        public IList<int> Numbers { get; set; }
        public IList<object> Objects { get; set; }
        public DateTime? DateTime { get; set; }

        public string Decorator(string text)
        {
            return "-=" + text + "=-";
        }
    }
}