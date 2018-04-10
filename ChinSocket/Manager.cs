using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace ChinSocket
{
    public class Manager
    {
        private ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();
        private ConcurrentDictionary<string, string> _rooms = new ConcurrentDictionary<string, string>();

        public ConcurrentDictionary<string, WebSocket> WebSockets
        {
            get { return _sockets; }
        }

        public int ClientsConnected(string room)
        {
            return _rooms.Where(p => p.Value == room).Count();
        }

        public string GetSocketId(WebSocket socket)
        {
            return _sockets.FirstOrDefault(s => s.Value == socket).Key;
        }

        public string GetRoomById(string id)
        {
            return _rooms.FirstOrDefault(p => p.Key == id).Value;
        }

        public void AddSocket(WebSocket socket, string room)
        {
            string id = GenerateId();

            _sockets.TryAdd(id, socket);
            _rooms.TryAdd(id, room);
        }

        public async Task RemoveSocket(string id)
        {
            WebSocket socket;
            _sockets.TryRemove(id, out socket);

            string room;
            _rooms.TryRemove(id, out room);

            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, $"Closed {room} by SocketsManager", CancellationToken.None);
        }

        private string GenerateId()
        {
            return Guid.NewGuid().ToString().Substring(0,8);
        }
    }
}
