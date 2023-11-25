using Core.Network.Packets;
using Core.Streams;
using System.Diagnostics;

namespace ExodiumMC.Core.Network.Packets
{
    public struct Msg
    {
        private readonly PacketIds _id;
        private readonly byte[] _data;
        private readonly PacketReader _reader;
        public Msg(byte[] data)
        {
            _data = data;
            _reader = new PacketReader(new MemoryStream(_data));
            _id = (PacketIds)_reader.ReadInt();
#if DEBUG 
            Console.WriteLine($"IN {_id} data: {BitConverter.ToString(_data)}");
#endif
        }
        public Msg(int id, byte[] data)
        {
            _id = (PacketIds)id;
            _data = data;
            _reader = new PacketReader(new MemoryStream(_data));
#if DEBUG
            Console.WriteLine($"IN {_id} data: {BitConverter.ToString(_data)}");
#endif
        }
        public PacketIds GetId() => _id;
        public PacketReader GetReader() => _reader;
    }
}
