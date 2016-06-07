using System;
using RazorEngine.Templating;

namespace webmetal
{
    public class WebMetalCachingProvider : ICachingProvider
    {

        private InvalidatingCachingProvider inner;
        public WebMetalApplication application;

        public WebMetalCachingProvider()
        {
            inner = new InvalidatingCachingProvider();
        }

        public TypeLoader TypeLoader
        {
            get
            {
                return inner.TypeLoader;
            }
        }

        public static Type GetModelTypeKey(Type modelType)
        {
            return InvalidatingCachingProvider.GetModelTypeKey(modelType);
        }

        public void CacheTemplate(ICompiledTemplate template, ITemplateKey templateKey)
        {
            inner.CacheTemplate(template, templateKey);
        }

        public bool TryRetrieveTemplate(ITemplateKey templateKey, Type modelType, out ICompiledTemplate compiledTemplate)
        {
            bool b = inner.TryRetrieveTemplate(templateKey, modelType, out compiledTemplate);

            if (application.debugMode)
                inner.InvalidateCache(templateKey);

            return b;

        }

        public void Dispose()
        {
            inner.Dispose();
        }
    }
}