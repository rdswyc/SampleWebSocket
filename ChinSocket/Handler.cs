using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ChinSocket
{
    public class Handler
    {
        private Manager _manager { get; set; }

        public Handler(Manager manager)
        {
            _manager = manager;
        }

        public async Task OnConnected(WebSocket socket, string room)
        {
            _manager.AddSocket(socket, room);

            string socketId = _manager.GetSocketId(socket);

            Message message = new Message(socketId, _manager.ClientsConnected(room), Message.Types.Openning, room);
            await SendMessageToAllAsync(JsonConvert.SerializeObject(message), room);
        }

        public async Task OnDisconnected(WebSocket socket)
        {
            string socketId = _manager.GetSocketId(socket);
            string room = _manager.GetRoomById(socketId);

            Message message = new Message(socketId, _manager.ClientsConnected(room) - 1, Message.Types.Closing, room);
            await SendMessageToAllAsync(JsonConvert.SerializeObject(message), room);

            await _manager.RemoveSocket(socketId);
        }

        public async Task SendMessageAsync(WebSocket socket, string message)
        {
            if(socket.State != WebSocketState.Open) return;

            var buffer = Encoding.UTF8.GetBytes(message);
            await socket.SendAsync(new ArraySegment<byte>(buffer,0,message.Length), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task SendMessageToAllAsync(string message, string room)
        {
            foreach(var pair in _manager.WebSockets)
            {
                WebSocket socket = pair.Value;

                if (socket.State == WebSocketState.Open && _manager.GetRoomById(pair.Key) == room)
                {
                    await SendMessageAsync(socket, message);
                }
            }
        }

        public async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            string socketId = _manager.GetSocketId(socket);
            string room = _manager.GetRoomById(socketId);

            string content = Encoding.UTF8.GetString(buffer, 0, result.Count);

            Message message = new Message(socketId, content);
            await SendMessageToAllAsync(JsonConvert.SerializeObject(message), room);
        }
    }
}