using Core.Async;
using Core.Logging;
using Core.Network.Packets;
using ExodiumMC.Core.Network.Packets;
using System.Collections.Concurrent;
using UserInterface;

namespace Core.Network.Server
{
    public static class ConnectionManager
    {
        private readonly static Listener listener = new(new mAddr(Env.serverHost));
        private readonly static BlockingCollection<Client> _clients = new();
        private readonly static TaskPool _taskPool = new(10);
        public static void Start()
        {
            
            listener.Listen(OnAccept);
        }
        public static void OnMessage(Client client, Msg msg)
        {
            _taskPool.PendTask(() => { Routing.InvokeRoute(client, msg); });
        }
        public static void OnAccept(Client client)
        {
            _clients.Add(client);
            _taskPool.IncreasePool(3);

            client.SetOnDisconnect(RemoveClient);
            client.OnMessage(OnMessage);
        }
        public static void RemoveClient(Client client)
        {
            _clients.ToList().Remove(client);
            _taskPool.ShrinkPool(3);
        }
    }
}
