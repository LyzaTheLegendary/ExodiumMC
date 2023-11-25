using Core.Async;
using Core.Logging;
using Core.Network.Packets;
using Core.Streams;
using ExodiumMC.Core.Network.Packets;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace Core.Network
{
    public enum ConnState : byte
    {
        UNKNOWN = 0,
        STATUS = 1,
        LOGIN = 2,
        PLAY = 3,
        DISCONNECTED = 4,
    }
    public class Client
    {
        public ConnState state = ConnState.UNKNOWN;
        private readonly BlockingCollection<byte[]> _outPool = new();
        private readonly BlockingCollection<Msg> _inPool = new();
        private readonly mAddr _addr;
        private readonly Socket _socket;
        private readonly TaskPool _taskPool = new(4);
        private readonly PacketReader mcReader;
        private Action<Client>? _onDisconnect;
        public Client(Socket socket)
        {
            _socket = socket;
            _addr = new mAddr(_socket.RemoteEndPoint!.ToString()!);
            mcReader = new PacketReader(new NetworkStream(_socket));
            
            _socket.ReceiveTimeout = 999;
            try
            {
                int size = mcReader.ReadInt() - 1; // disguisting Hotfix for new mc problem?

                int id = mcReader.ReadInt();
                byte[] buff = mcReader.ReadBytes(size);

                HandShake handShake = new(new Msg(id, buff));

                if (handShake.state == ConnState.STATUS)
                {
                    state = handShake.state;
                    PacketWriter writer = new(0);
                    writer.WriteString($"{{\"version\":{{\"name\":\"SharpCraft\",\"protocol\":{Env.serverVer}}},\"enforcesSecureChat\":false,\"description\":{{\"text\":\"Exodium Development\"}},\"players\":{{\"max\":{100},\"online\":0}},\"preventsChatReports\":true}}");
                    _socket.Send((byte[])writer);
                    byte[] buff2 = mcReader.ReadBytes(11);
                    _socket.Send(buff2.TakeLast(10).ToArray());
                    Disconnect("Magic packet that fixes it?");
                }
                else if (handShake.state == ConnState.LOGIN)
                {
                    state = handShake.state;
                    if (handShake.protocolVer != Env.serverVer)
                    {
                        Disconnect("Invalid version!");
                        return;
                    }

                }
                else
                {
                    Disconnect();
                    return;
                }
            }
            catch (SocketException e) { 
                Disconnect(); 
                return; }
            catch (Exception e)     { LoggingService.WriteError(e.Message); }

            if (state == ConnState.LOGIN)
            {
                SendMessages();
                Listen();
            }
        }
        private void Listen()
        {
            _taskPool.PendTask(() =>
            {
                while (_socket.Connected)
                {
                    try
                    {
                        if (_socket.Receive(new byte[2], SocketFlags.Peek) == 0)
                        {
                            Disconnect();
                            return;
                        }
                        //Reads size as a leb128 int and then reads that amount in bytes and sends it off to the pool
                        _inPool.Add(new Msg(mcReader.ReadBytes(mcReader.ReadInt())));
                    }
                    catch (SocketException e) {
                        if (e.NativeErrorCode == 10060) {
                            Disconnect($"Response too slow, Took more than {Env.receiveDelay}ms");
                        }
                        else { Disconnect($"Server Exception: {e.Message}"); }
                    }
                }
            });
        }
        private void SendMessages()
        {
            _taskPool.PendTask(() =>
            {
                foreach(byte[] data in _outPool.GetConsumingEnumerable()) {
                    _socket.Send(data);
                }
            });
        }
        public void OnMessage(Action<Client,Msg> process)
        {
            _taskPool.PendTask(() =>
            {
                foreach(Msg msg in _inPool.GetConsumingEnumerable())
                    process(this,msg);
            });
        }
        public void PendMessage(byte[] msg) => _outPool.Add(msg);
        public void Disconnect(string? reason = null)
        {
            if(state == ConnState.DISCONNECTED) return;
            state = ConnState.DISCONNECTED;

            if (reason != null)
            {
                PacketWriter writer = new PacketWriter(0);
                writer.WriteString($"{{\r\n    \"text\": \"{reason}\"\r\n}}");
                _socket.Send((byte[])writer);

                _onDisconnect?.Invoke(this);
                _taskPool.Stop();
            }
            else
            {
                _onDisconnect?.Invoke(this);
                _taskPool.Stop();
            }

            _socket.Close();
            
        }
        public mAddr GetAddr() => _addr;
        public void SetOnDisconnect(Action<Client> onDisconnected) 
            => _onDisconnect = onDisconnected;

    }
}
