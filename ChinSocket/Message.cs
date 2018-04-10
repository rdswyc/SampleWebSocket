using Newtonsoft.Json;

namespace ChinSocket
{
    public class Message
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string SocketID { get; set; }

        [JsonProperty("clients", NullValueHandling = NullValueHandling.Ignore)]
        public int? Clients { get; set; }

        [JsonProperty("event", NullValueHandling = NullValueHandling.Ignore)]
        public Types? EventType { get; set; }

        [JsonProperty("content", NullValueHandling = NullValueHandling.Ignore)]
        public string Content { get; set; }

        [JsonProperty("room", NullValueHandling = NullValueHandling.Ignore)]
        public string Room { get; set; }

        public enum Types
        {
            Data = 0,
            Openning = 1,
            Closing = -1
        }

        public Message(string socketID, int clients, Types type, string room, string content = null)
        {
            this.SocketID = socketID;
            this.Clients = clients;
            this.EventType = type;
            this.Room = room;
            if (content != null) this.Content = content;
        }

        public Message(string socketID, string content)
        {
            this.SocketID = socketID;
            this.Content = content;
        }
    }
}
