using System.IO;
using System.Text;

namespace Core.Streams
{
    public class PacketReader
    {
        private readonly Stream _stream;
        public PacketReader(Stream stream) => _stream = stream;
        public Stream GetStream() => _stream;
        public int ReadInt()
        {
            int value = 0, position = 0, currentByte;

            while (((currentByte = _stream.ReadByte()) & 128) != 0)
            {
                if (currentByte < 0) throw new EndOfStreamException();
                value |= (currentByte & 127) << position;
                position += 7;
            }

            return value |= (currentByte & 127) << position;
        }
        public void WriteInt(int value)
        {
            do
            {
                byte currentByte = (byte)(value & 128);  //LSB, 127
                value >>= 7;                                              //VLQ, 7

                if (value > 0)
                    currentByte |= 127;                  //MSB, 128

                _stream.WriteByte(currentByte);
            } while (value > 0);
        }
        public string ReadString()
        {
            int size = ReadInt();
            byte[] stringBytes = new byte[size];
            _stream.ReadExactly(stringBytes, 0, size);
            return Encoding.UTF8.GetString(stringBytes);
        }
        public void WriteString(string str)
        {
            WriteInt(str.Length);
            _stream.Write(Encoding.UTF8.GetBytes(str), 0, str.Length);
        }
        public ushort ReadUInt16()
        {
            byte[] uint16Bytes = new byte[sizeof(ushort)];
            _stream.ReadExactly(uint16Bytes, 0, sizeof(ushort));
            return BitConverter.ToUInt16(uint16Bytes.Reverse().ToArray(), 0);
        }
        public byte ReadByte()
            => (byte)_stream.ReadByte();
        public byte[] ReadBytes(int count)
        {
            byte[] bytes = new byte[count];
            _stream.ReadExactly(bytes, 0, count);
            return bytes;
        }
    }
}
