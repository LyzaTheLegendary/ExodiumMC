using ExodiumMC.Core;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Core.Network
{
    public class SRemoteConsole
    {
        private readonly Socket _listenerSock;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        public SRemoteConsole()
        {
            mAddr addr = new(Env.reconHost);
            IPEndPoint ep = new(IPAddress.Parse(addr.GetHost()), addr.GetPort());
            _listenerSock = new Socket(ep.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _listenerSock.Bind(ep);
            _listenerSock.Listen(1);
        }
        public void Stop() => _cancellationTokenSource.Cancel();
        public void Start()
        {
            Task.Run(() => { 
                RESTART:
                Socket socket = _listenerSock.Accept();
                byte[] pass = new byte[1024];
                try
                { socket.Receive(pass); }
                catch (SocketException)
                { socket.Close(); goto RESTART; }

                string password = Encoding.UTF8.GetString(pass, 0, pass.Length).TrimEnd('\0');
                if (!(password == Env.reconPass))
                {
                    socket.Close();
                    goto RESTART;
                }

                while (true)
                {
                    byte[] buff = new byte[1024];
                    try
                    {
                        socket.Receive(buff);
                    }
                    catch (SocketException)
                    {
                        socket.Close();
                        goto RESTART;
                    }

                    string cmd = Encoding.UTF8.GetString(buff, 0, buff.Length);
                    string response = CommandParser.HandleCMD(cmd.TrimEnd('\0'));

                    try { socket.Send(Encoding.UTF8.GetBytes(response)); } catch (SocketException) { socket.Close(); goto RESTART; };
                }
            }, _cancellationTokenSource.Token);
        }
    }
}
