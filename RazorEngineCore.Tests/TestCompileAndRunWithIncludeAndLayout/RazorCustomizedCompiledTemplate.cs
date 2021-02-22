using System.Collections.Generic;

namespace RazorEngineCore.Tests
{
    public class RazorCustomizedCompiledTemplate
    {
        private readonly IRazorEngineCompiledTemplate<RazorIncludeAndLayoutTemplateBase> compiledTemplate;
        private readonly Dictionary<string, IRazorEngineCompiledTemplate<RazorIncludeAndLayoutTemplateBase>> compiledParts;

        public RazorCustomizedCompiledTemplate(IRazorEngineCompiledTemplate<RazorIncludeAndLayoutTemplateBase> compiledTemplate, Dictionary<string, IRazorEngineCompiledTemplate<RazorIncludeAndLayoutTemplateBase>> compiledParts)
        {
            this.compiledTemplate = compiledTemplate;
            this.compiledParts = compiledParts;
        }

        public string Run(object model)
        {
            return this.Run(this.compiledTemplate, model);
        }

        public string Run(IRazorEngineCompiledTemplate<RazorIncludeAndLayoutTemplateBase> template, object model)
        {
            RazorIncludeAndLayoutTemplateBase templateReference = null;

            string result = template.Run(instance =>
            {
                if (!(model is AnonymousTypeWrapper))
                {
                    model = new AnonymousTypeWrapper(model);
                }

                instance.Model = model;
                instance.IncludeCallback = (key, includeModel) => this.Run(this.compiledParts[key], includeModel);

                templateReference = instance;
            });

            if (templateReference.Layout == null)
            {
                return result;
            }

            return this.compiledParts[templateReference.Layout].Run(instance =>
            {
                if (!(model is AnonymousTypeWrapper))
                {
                    model = new AnonymousTypeWrapper(model);
                }
                instance.Model = model;
                instance.IncludeCallback = (key, includeModel) => this.Run(this.compiledParts[key], includeModel);
                instance.RenderBodyCallback = () => result;
            });
        }
    }
}