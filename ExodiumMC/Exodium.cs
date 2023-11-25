using Core;
using Core.Network.Packets;
using Core.Network.Server;
using UserInterface;

internal class Exodium
{
#pragma warning disable CS8618 
    private static Thread logicThread;
#pragma warning restore CS8618 
    [STAThread]
    public static void Main()
    {
        Env.Construct();
        logicThread = new Thread(StartUp);
        logicThread.Start();
        Display.Construct();

    }
    public static void StartUp()
    {
        Thread.Sleep(100);
        Globals.remoteConsole = new Core.Network.SRemoteConsole();
        Globals.remoteConsole.Start();
        Routing.AddRoute(PacketIds.Login,PacketProcessor.OnLogin);
        ConnectionManager.Start();
    }
}