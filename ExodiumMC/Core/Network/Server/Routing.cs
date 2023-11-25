using Core.Network.Packets;
using ExodiumMC.Core.Network.Packets;
using System.Collections.Concurrent;

namespace Core.Network.Server
{
    public class Routing
    {
        private static ConcurrentDictionary<PacketIds, Action<Client, Msg>> routes = new();
        public static void AddRoute(PacketIds packetId, Action<Client, Msg> action) 
            => routes[packetId] = action;
        public static void InvokeRoute(Client client,Msg msg) { 
            routes.TryGetValue(msg.GetId(), out Action<Client, Msg>? action);
            action?.Invoke(client, msg);
        }
    }
}
