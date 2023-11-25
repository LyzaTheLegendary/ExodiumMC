using Core.Streams;
using ExodiumMC.Core.Network.Packets;

namespace Core.Network.Packets
{
    public enum PacketIds : int
    {
        Login = 0,
    }
    public readonly struct HandShake
    {
        public readonly int protocolVer;
        public readonly mAddr endpoint;
        public readonly ConnState state;
        internal HandShake(Msg msg)
        {
            PacketReader stream = msg.GetReader();
            protocolVer = stream.ReadInt();
            endpoint = new mAddr(stream.ReadString() + ":" + stream.ReadUInt16().ToString());
            state = (ConnState)stream.ReadByte();
        }
    }
    public readonly struct Login
    {
        public readonly string username;
        public readonly byte[] uuid;
        public Login(Msg msg)
        {
            PacketReader stream = msg.GetReader();
            username = stream.ReadString();
            uuid = stream.ReadBytes(16);
        }
    }
}
