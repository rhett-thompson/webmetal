using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.SessionState;
using RazorEngine.Templating;

namespace webmetal
{

    public class WebMetalModel : Attribute
    {

    }

    public class RequestBodyModel : WebMetalModel
    {

        internal string mimeType;

        public RequestBodyModel(string mimeType = null)
        {
            this.mimeType = mimeType;
        }

    }

    public class QueryStringModel : WebMetalModel
    {


    }

    public class FormFieldsModel : WebMetalModel
    {


    }

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

            string requestContentType = context.Request.ContentType.ToLower().Trim();

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

                    bool isModel = param.GetCustomAttribute<WebMetalModel>(true) != null;

                    if (!isModel)
                    {
                        if (context.Request.Form[param.Name] != null)
                            methodParams.Add(Utility.ChangeType(context.Request.Form[param.Name], param.ParameterType));
                        else if (context.Request.QueryString[param.Name] != null)
                            methodParams.Add(Utility.ChangeType(context.Request.QueryString[param.Name], param.ParameterType));
                        else
                            methodParams.Add(param.ParameterType.IsValueType ? Activator.CreateInstance(param.ParameterType) : null);
                    }
                    else
                    {

                        QueryStringModel queryStringModel = param.GetCustomAttribute<QueryStringModel>(true);
                        FormFieldsModel formFieldsModel = param.GetCustomAttribute<FormFieldsModel>(true);
                        RequestBodyModel requestBodyModel = param.GetCustomAttribute<RequestBodyModel>(true);

                        if (queryStringModel != null)
                            methodParams.Add(Utility.MapCollectionToObject(context.Request.QueryString, param.ParameterType));
                        else if (formFieldsModel != null)
                            methodParams.Add(Utility.MapCollectionToObject(context.Request.Form, param.ParameterType));
                        else if (requestBodyModel != null)
                        {

                            if (!string.IsNullOrEmpty(requestBodyModel.mimeType))
                                requestContentType = requestBodyModel.mimeType;

                            if (!application.mimeTypeHandlers.ContainsKey(requestContentType))
                                throw new NotImplementedException(string.Format("A mime type handler for '{0}' has not been implemented.", requestContentType));

                            MimeTypeHandler requestMimeHandler = application.mimeTypeHandlers[requestContentType];

                            methodParams.Add(requestMimeHandler.deserialize(context.Request.InputStream, param.ParameterType));

                        }

                    }

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

        public string view(string source, object model = null, IDictionary<string, object> viewBag = null)
        {

            return application.view(source, model, viewBag);

        }

    }

}