using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ChinSocket
{
    public class Middleware
    {
        private readonly RequestDelegate _next;
        private Handler _handler { get; set; }

        public Middleware(RequestDelegate next, Handler handler)
        {
            _next = next;
            _handler = handler;
        }

        public async Task Invoke(HttpContext context)
        {
            if(!context.WebSockets.IsWebSocketRequest) return;
            
            WebSocket socket = await context.WebSockets.AcceptWebSocketAsync();

            await _handler.OnConnected(socket, context.Request.Query["room"]);

            await Receive(socket, async(result, buffer) =>
            {
                if(result.MessageType == WebSocketMessageType.Text)
                {
                    await _handler.ReceiveAsync(socket, result, buffer);
                    return;
                }
                else if(result.MessageType == WebSocketMessageType.Close)
                {
                    await _handler.OnDisconnected(socket);
                    return;
                }
            });
        }

        public async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];

            while(socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                handleMessage(result, buffer);                
            }
        }
    }
}
