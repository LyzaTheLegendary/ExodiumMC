using Core.Logging;
using System.Net;
using System.Net.Sockets;

namespace Core.Network.Server
{
    public class Listener
    {
        private readonly mAddr _host;
        private readonly Socket _listenerSock;
        public Listener(mAddr host, byte listenQueue = 10)
        {
            _host = host;
            IPEndPoint ep = new(IPAddress.Parse(host.GetHost()),host.GetPort());
            _listenerSock = new Socket(ep.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _listenerSock.Bind(ep);
            _listenerSock.Listen(listenQueue);
        }
        public mAddr GetAddr() => _host;
        public void Listen(Action<Client> onAccept)
        {
            Task.Run(() =>
            {
                LoggingService.WriteInfo($"Listening for new connections on : {GetAddr()}");
                while (true)
                {
                    onAccept(new Client(_listenerSock.Accept()));
                }
            });
        }
    }
}
