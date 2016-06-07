using System;
using System.Collections.Generic;
using System.IO;
using RazorEngine.Templating;

namespace webmetal
{

    public class WebMetalResolvePathTemplateManager : ITemplateManager
    {
        private readonly IEnumerable<string> layoutRoots;

        public WebMetalResolvePathTemplateManager(IEnumerable<string> layoutRoots)
        {
            this.layoutRoots = layoutRoots;
        }

        public ITemplateSource Resolve(ITemplateKey key)
        {
            var full = key as FullPathTemplateKey;
            if (full == null)
                throw new NotSupportedException("You can only use FullPathTemplateKey with this manager");

            var template = File.ReadAllText(full.FullPath);
            return new LoadedTemplateSource(template, full.FullPath);

        }

        public ITemplateKey GetKey(string name, ResolveType resolveType, ITemplateKey context)
        {
            return new FullPathTemplateKey(name, ResolveFilePath(name), resolveType, context);
        }

        protected string ResolveFilePath(string name)
        {
            if (File.Exists(name))
                return name;

            foreach (var root in layoutRoots)
            {

                var path = Path.Combine(root, name);

                if (File.Exists(path))
                    return path;

                path += ".cshtml";
                if (File.Exists(path))
                    return path;

            }

            throw new InvalidOperationException(string.Format("Could not resolve template {0}", name));

        }

        public void AddDynamic(ITemplateKey key, ITemplateSource source)
        {
            throw new NotSupportedException("Adding templates dynamically is not supported! Instead you probably want to use the full-path in the name parameter?");
        }

    }

}