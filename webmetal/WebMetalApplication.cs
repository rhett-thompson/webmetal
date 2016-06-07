using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.Routing;
using RazorEngine.Configuration;
using RazorEngine.Templating;

namespace webmetal
{
    public class WebMetalApplication : HttpApplication
    {

        public List<string> ignoredMethods = new List<string>() { "render", "init" };
        public List<string> specialPagePrefixList = new List<string>() { "page" };
        public Dictionary<string, Type> pages = new Dictionary<string, Type>();
        public Dictionary<string, MethodInfo> methods = new Dictionary<string, MethodInfo>();
        public Dictionary<string, string> fileDataCache = new Dictionary<string, string>();
        public Dictionary<string, MimeTypeHandler> mimeTypeHandlers = new Dictionary<string, MimeTypeHandler>();

        public IRazorEngineService razorService;
        //public TemplateServiceConfiguration razorConfig;
        //public InvalidatingCachingProvider razorDebugCacheProvider;


#if DEBUG
        public bool debugMode = true;
#else
        public bool debugMode = false;
#endif

        public void init(Type rootPage, IEnumerable<string> layoutRoots = null, Assembly assembly = null)
        {

            if (layoutRoots == null)
                layoutRoots = new string[] { HttpRuntime.AppDomainAppPath };

            TemplateServiceConfiguration razorConfig = new TemplateServiceConfiguration()
            {
                Debug = debugMode,
                BaseTemplateType = typeof(WebMetalTemplate<>)
            };

            razorConfig.TemplateManager = new WebMetalResolvePathTemplateManager(layoutRoots);
            razorConfig.CachingProvider = new WebMetalCachingProvider() { application = this };

            razorService = RazorEngineService.Create(razorConfig);

            if (assembly == null)
                assembly = Assembly.GetCallingAssembly();

            addPage(rootPage, "");

            foreach (Type type in assembly.GetTypes())
            {

                if (!type.IsSubclassOf(typeof(Page)) || type.FullName.StartsWith("webmetal"))
                    continue;

                string baseRoute = type.Name;

                RouteConfig routeConfig = type.GetCustomAttribute<RouteConfig>(false);
                if (routeConfig != null)
                {

                    if (routeConfig.ignore)
                        continue;

                    if (routeConfig.useNamespaceForBaseRoute)
                        baseRoute = type.FullName.Replace('.', '/').ToLower();

                    if (routeConfig.overrideRoute != null)
                        baseRoute = routeConfig.overrideRoute;

                }

                //add page
                addPage(type, baseRoute.ToLower());

            }

            mimeTypeHandlers.Add("text/plain", new TextPlainMimeTypeHandler());

        }

        private void addPage(Type type, string baseRoute)
        {

            //remove special prefixes
            foreach (string prefix in specialPagePrefixList)
                if (baseRoute.EndsWith(prefix))
                    baseRoute = baseRoute.Substring(0, baseRoute.Length - prefix.Length);

            //create route handler
            RouteHandler routeHandler = new RouteHandler()
            {

                application = this,
                pageType = type

            };

            //add base route to page
            RouteTable.Routes.Add(Guid.NewGuid().ToString("n"), new Route(baseRoute, routeHandler));

            //add methods
            foreach (MethodInfo method in type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {

                if (ignoredMethods.Contains(method.Name))
                    continue;

                string methodRoute = string.Format("{0}/{1}", baseRoute, method.Name.ToLower()).TrimStart(new char[] { '/' });

                RouteTable.Routes.Add(new Route(methodRoute, routeHandler));
                methods.Add(methodRoute, method);

            }

            //add page to cache
            pages.Add(baseRoute, type);


        }

        public virtual string view(string source, object model = null, IDictionary<string, object> viewBag = null)
        {
 
            return razorService.RunCompile(source, null, model);


        }

    }

}