using System;
using System.Web;
using System.Web.Routing;
using webmetal;

namespace WebApplication1
{
    public class Global : WebMetalApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            init(typeof(Home), new string[] { HttpContext.Current.Server.MapPath("~/templates") });

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}