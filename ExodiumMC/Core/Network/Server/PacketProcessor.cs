using Core.Extensions;
using Core.Network.Packets;
using Core.Streams;
using ExodiumMC.Core.Network.Packets;
using UserInterface;

namespace Core.Network.Server
{
    public static class PacketProcessor
    {
        public static void OnLogin(Client client, Msg msg)
        {
            Login handShake = new(msg);

            Display.Write(handShake.username);
        }
    }
}
