using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.SessionState;

namespace webmetal
{

    public class RouteConfig : Attribute
    {

        internal string overrideRoute;
        internal bool useNamespaceForBaseRoute;
        internal bool ignore;

        public RouteConfig(string OverrideRoute = null, bool UseNamespaceForBaseRoute = false, bool Ignore = false)
        {
            overrideRoute = OverrideRoute;
            useNamespaceForBaseRoute = UseNamespaceForBaseRoute;
            ignore = Ignore;
        }
    }

    public abstract class Page : IHttpHandler, IRequiresSessionState
    {

        public WebMetalApplication application;

        public HttpContext context;
        public HttpRequest request;
        public HttpResponse response;
        public HttpServerUtility server;
        
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public void ProcessRequest(HttpContext context)
        {

            this.context = context;
            request = context.Request;
            response = context.Response;
            server = context.Server;
            
            init();

            string path = context.Request.Path.ToLower().TrimEnd(new char[] { '/' }).TrimStart(new char[] { '/' });

            if (!application.pages.ContainsKey(path) && !application.methods.ContainsKey(path))
                return;

            if (application.methods.ContainsKey(path))
            {

                MethodInfo method = application.methods[path];
                List<object> methodParams = new List<object>();

                foreach (ParameterInfo param in method.GetParameters())
                {

                    if (context.Request.Form[param.Name] != null)
                        methodParams.Add(Utility.ChangeType(context.Request.Form[param.Name], param.ParameterType));
                    else if (context.Request.QueryString[param.Name] != null)
                        methodParams.Add(Utility.ChangeType(context.Request.QueryString[param.Name], param.ParameterType));
                    else
                        methodParams.Add(param.ParameterType.IsValueType ? Activator.CreateInstance(param.ParameterType) : null);

                }

                method.Invoke(this, methodParams.ToArray());

            }
            else
                render();
            
        }

        public virtual void init() { }

        public abstract void render();

        public string loadFileData(string source)
        {

            return application.loadFileData(source);

        }

    }

}