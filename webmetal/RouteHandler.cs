using System;
using System.Web;
using System.Web.Routing;

namespace webmetal
{
    internal class RouteHandler : IRouteHandler
    {

        public Type pageType;
        public WebMetalApplication application;

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            Page page = Activator.CreateInstance(pageType) as Page;
            page.application = application;

            return page;
        }

    }
}