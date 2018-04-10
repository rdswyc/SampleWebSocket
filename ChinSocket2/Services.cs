using System;
using System.IO;
using System.Text;
using System.Web;

namespace ChinSocket2
{
    public class LogService
    {
        public static void LogEvent(Message content)
        {
            string path = HttpContext.Current.Server.MapPath("~/events.txt");

            if (content.EventType == "open") content.EventType += " ";

            using (StreamWriter stream = new StreamWriter(path, true, Encoding.UTF8))
            {
                stream.Write($"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}");
                stream.Write(" | ");
                stream.Write($"Event: {content.EventType}");
                stream.Write(" | ");
                stream.Write($"Id: {content.ID}");
                stream.Write(" | ");
                stream.Write($"Room: {content.Room}");
                stream.Write(" | ");
                stream.WriteLine($"Clients: {content.Clients}");
                stream.Close();
            }
        }

        public static void LogContent(Message content)
        {
            string path = HttpContext.Current.Server.MapPath("~/messages.txt");

            using (StreamWriter stream = new StreamWriter(path, true, Encoding.UTF8))
            {
                stream.Write($"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}");
                stream.Write(" | ");
                stream.Write($"Id: {content.ID}");
                stream.Write(" | ");
                stream.Write($"Room: {content.Room}");
                stream.Write(" | ");
                stream.WriteLine($"Clients: {content.Clients}");
                stream.WriteLine("--");
                stream.WriteLine(content.Content);
                stream.WriteLine("--");
                stream.Close();
            }
        }
    }
}