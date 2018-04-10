using Newtonsoft.Json;
using System;
using System.Net.WebSockets;

namespace ChinSocket2
{
    public class MySocket
    {
        public WebSocket WebSocket { get; set; }
        public string Id { get; set; }
        public string Room { get; set; }

        public MySocket(WebSocket socket, string room)
        {
            this.Id = GenerateId();
            this.WebSocket = socket;
            this.Room = room;
        }

        private string GenerateId()
        {
            return Guid.NewGuid().ToString().Substring(0, 8);
        }
    }

    public class Message
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string ID { get; set; }

        [JsonProperty("event", NullValueHandling = NullValueHandling.Ignore)]
        public string EventType { get; set; }

        [JsonProperty("room", NullValueHandling = NullValueHandling.Ignore)]
        public string Room { get; set; }

        [JsonProperty("clients", NullValueHandling = NullValueHandling.Ignore)]
        public int? Clients { get; set; }

        [JsonProperty("content", NullValueHandling = NullValueHandling.Ignore)]
        public string Content { get; set; }
        
        public Message(string id, int clients, string type, string room = null, string content = null)
        {
            this.ID = id;
            this.Clients = clients;
            this.EventType = type;
            this.Room = room;
            this.Content = content;
        }

        public Message(string socketID, string content)
        {
            this.ID = socketID;
            this.Content = content;
        }
    }
}