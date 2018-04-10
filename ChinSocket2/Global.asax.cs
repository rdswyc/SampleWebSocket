using System;
using System.Web;
using System.Web.Routing;

namespace ChinSocket2
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            RouteCollection routes = RouteTable.Routes;
            routes.Add(new Route("ws", new SocketRouteHandler()));
            routes.Add(new Route("clear", new SocketClearRouteHandler()));
        }
    }
}