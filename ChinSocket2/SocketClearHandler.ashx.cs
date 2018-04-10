using System;
using System.Web;
using System.Web.Routing;

namespace ChinSocket2
{
    public class SocketClearHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write(ClearAll());
        }

        public static string ClearAll()
        {
            string response = String.Empty;

            foreach (var client in SocketHandler.Clients)
            {
                try
                {
                    client.WebSocket.Abort();
                    response += $"closed socket {client.Id}\r\n";
                }
                catch (Exception)
                {
                }
            }

            SocketHandler.Locker.EnterWriteLock();
            SocketHandler.Clients.Clear();
            SocketHandler.Locker.ExitWriteLock();

            return $"{response}\r\nclosed all\r\n";
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }

    public class SocketClearRouteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext context)
        {
            return new SocketClearHandler();
        }
    }
}