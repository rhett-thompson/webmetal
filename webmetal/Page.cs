using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.Routing;
using System.Web.SessionState;

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

    public class RouteDataModel : WebMetalModel
    {

    }

    public class OverrideName : Attribute
    {

        internal string overrideName;

        public OverrideName(string OverrideName)
        {
            overrideName = OverrideName;
        }

    }

    public class Ignore : Attribute
    {

    }

    public class SkipInit : Attribute
    {

    }

    public class CustomRoute : Attribute
    {

        internal string route;

        public CustomRoute(string Route)
        {
            route = Route;
        }

    }

    public abstract class Page : IHttpHandler, IRequiresSessionState
    {

        public WebMetalApplication application;

        public HttpContext context;
        public HttpRequest request;
        public HttpResponse response;
        public HttpServerUtility server;
        public RouteData routeData;

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public void ProcessRequest(HttpContext context)
        {

            try
            {

                string requestContentType = context.Request.ContentType.ToLower().Trim();

                this.context = context;
                request = context.Request;
                response = context.Response;
                server = context.Server;
                routeData = request.RequestContext.RouteData;
                
                string route = (routeData.Route as Route).Url;

                if (application.pages.ContainsKey(route))
                {
                    init();
                    index();
                }
                else if (application.methods.ContainsKey(route))
                {

                    MethodInfo method = application.methods[route];

                    if (method.GetCustomAttribute<SkipInit>() == null)
                        init();

                    List<object> methodParams = new List<object>();
                    foreach (ParameterInfo param in method.GetParameters())
                    {

                        bool isModel = param.GetCustomAttribute<WebMetalModel>(true) != null;

                        if (!isModel)
                        {
                            if (request.Form[param.Name] != null)
                                methodParams.Add(Utility.ChangeType(request.Form[param.Name], param.ParameterType));
                            else if (request.QueryString[param.Name] != null)
                                methodParams.Add(Utility.ChangeType(request.QueryString[param.Name], param.ParameterType));
                            else if (routeData.Values[param.Name] != null)
                                methodParams.Add(Utility.ChangeType(routeData.Values[param.Name], param.ParameterType));
                            else
                                methodParams.Add(param.ParameterType.IsValueType ? Activator.CreateInstance(param.ParameterType) : null);
                        }
                        else
                        {

                            QueryStringModel queryStringModel = param.GetCustomAttribute<QueryStringModel>(true);
                            FormFieldsModel formFieldsModel = param.GetCustomAttribute<FormFieldsModel>(true);
                            RequestBodyModel requestBodyModel = param.GetCustomAttribute<RequestBodyModel>(true);
                            RouteDataModel routeDataModel = param.GetCustomAttribute<RouteDataModel>(true);

                            if (queryStringModel != null)
                                methodParams.Add(Utility.MapCollectionToObject(context.Request.QueryString, param.ParameterType));
                            else if (formFieldsModel != null)
                                methodParams.Add(Utility.MapCollectionToObject(context.Request.Form, param.ParameterType));
                            else if (routeDataModel != null)
                                methodParams.Add(Utility.MapCollectionToObject(routeData.Values, param.ParameterType));
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

                    object ret = method.Invoke(this, methodParams.ToArray());
                    if (ret != null)
                        response.Write(ret);

                }

            }
            catch (Exception ex)
            {
                error(ex);
            }

        }

        public virtual void init() { }

        public abstract void index();
        
        public string view(string source, object model, IDictionary<string, object> viewBag = null)
        {

            return application.view(source, model, viewBag);

        }

        public virtual void error(Exception ex)
        {

            throw ex;

        }

    }

}