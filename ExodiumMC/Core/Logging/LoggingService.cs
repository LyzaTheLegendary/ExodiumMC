using UserInterface;

namespace Core.Logging
{
    public static class LoggingService
    {
        public static object objLock = new();
        public static string currentTime { get { return DateTime.Now.ToString("yy-MM-dd"); } }
        public static void WriteInfo(string message)
            => Write($"{currentTime} [Thread:{Thread.CurrentThread.ManagedThreadId}] [INFO] " + message);
        public static void WriteError(string message)
            => Write($"{currentTime} [Thread:{Thread.CurrentThread.ManagedThreadId}] [ERR] " + message);
        
        private static void Write(string message)
        {
            lock (objLock)
            {
                Display.Write(message);
                File.AppendAllText("log.txt", message + Environment.NewLine);
            }
        }
    }
}
