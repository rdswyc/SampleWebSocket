using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using System.Web.WebSockets;

namespace ChinSocket2
{
    public class SocketHandler : IHttpHandler
    {
        public static readonly List<MySocket> Clients = new List<MySocket>();
        public static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim();

        private string room;

        public void ProcessRequest(HttpContext context)
        {
            room = context.Request.QueryString["room"];

            if (context.IsWebSocketRequest)
            {
                context.AcceptWebSocketRequest(ProcessSocketRequest);
            }
            else if (room != String.Empty)
            {
                context.Response.ContentType = "text/json";

                var clients = room == null ? Clients : Clients.Where(c => c.Room == room);

                if (clients.Count() == 0)
                {
                    context.Response.Write("{\"No clients\"}");
                }

                foreach (var client in clients)
                {
                    int roomCount = Clients.Where(c => c.Room == client.Room).Count();
                    var content = new Message(client.Id, roomCount, null, client.Room ?? String.Empty);

                    string message = JsonConvert.SerializeObject(content);
                    context.Response.Write(message + "\r\n");
                }
            }
        }

        private async Task ProcessSocketRequest(AspNetWebSocketContext context)
        {
            if (Clients.Count > 32) SocketClearHandler.ClearAll();

            var client = new MySocket(context.WebSocket, room);

            await SetEvent("open", client);

            while (client.WebSocket.State == WebSocketState.Open)
            {
                string receivedText = String.Empty;
                WebSocketReceiveResult result;
                var buffer = new ArraySegment<byte>(new byte[4096]);

                do
                {
                    result = await client.WebSocket.ReceiveAsync(buffer, CancellationToken.None);
                    var messageBytes = buffer.Skip(buffer.Offset).Take(result.Count).ToArray();
                    receivedText += Encoding.UTF8.GetString(messageBytes);
                }
                while (!result.EndOfMessage);
                
                if (result.MessageType == WebSocketMessageType.Text && receivedText != String.Empty)
                {
                    await SendMessage(new Message(client.Id, receivedText));
                }
                else
                {
                    break;
                }
            }

            await SetEvent("close", client);
        }

        public async Task SetEvent(string eventType, MySocket client)
        {
            Locker.EnterWriteLock();
            try
            {
                if (eventType == "open")
                {
                    Clients.Add(client);
                }
                else if (eventType == "close")
                {
                    Clients.Remove(client);
                    await client.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by the server", CancellationToken.None);
                }
            }
            finally
            {
                Locker.ExitWriteLock();
            }

            int roomCount = Clients.Where(c => c.Room == room).Count();
            var message = new Message(client.Id, roomCount, eventType, room);
            await SendMessage(message);
        }

        public async Task SendMessage(Message content)
        {
            string message = JsonConvert.SerializeObject(content);
            var buffer = Encoding.UTF8.GetBytes(message);

            var roomClients = Clients.Where(c => c.Room == room);

            foreach (var client in roomClients)
            {
                await client.WebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }

            if (content.Content == null)
            {
                LogService.LogEvent(content);
            }
            else
            {
                content.Room = room;
                content.Clients = roomClients.Count();
                LogService.LogContent(content);
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }

    public class SocketRouteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext context)
        {
            return new SocketHandler();
        }
    }
}