using Core.Network;
using Core.Streams;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Core.Extensions
{
    public static class ClientExtensions
    {
        public static void Kick(this Client client, string reason)
        {
            PacketWriter writer = new PacketWriter(0);
            writer.WriteString($"{{\r\n    \"text\": \"{reason}\"\r\n}}");
            client.PendMessage((byte[])writer);

            // Mc networking is poop, I hate this hack.
            Timer timer = new();
            timer.Elapsed += (object? obj, ElapsedEventArgs args) => { client.Disconnect(); }; ;
            timer.Interval = 1000;
            timer.AutoReset = false;
            timer.Start();
            
        }
    }
}
