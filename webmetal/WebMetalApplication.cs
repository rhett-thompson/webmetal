using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Routing;

namespace webmetal
{
    public class WebMetalApplication : HttpApplication
    {

        public List<string> ignoredMethods = new List<string>() { "render", "init" };
        public List<string> specialPagePrefixList = new List<string>() { "page" };
        public Dictionary<string, Type> pages = new Dictionary<string, Type>();
        public Dictionary<string, MethodInfo> methods = new Dictionary<string, MethodInfo>();
        public Dictionary<string, string> fileDataCache = new Dictionary<string, string>();

#if DEBUG
        public bool debugMode = true;
#else
        public bool debugMode = false;
#endif

        public void init(Type rootPage, Assembly assembly = null)
        {

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

        public virtual string loadFileData(string source)
        {

            if (debugMode)
                return File.ReadAllText(HttpContext.Current.Server.MapPath(source));

            if (fileDataCache.ContainsKey(source))
                return fileDataCache[source];
            else
            {
                fileDataCache[source] = File.ReadAllText(HttpContext.Current.Server.MapPath(source));
                return fileDataCache[source];
            }

        }

    }
    
}