namespace Core.Network
{
    public struct mAddr
    {
        private readonly string _host;
        private readonly ushort _port;
        public mAddr(string host)
        {
            string[] addrInfo = host.Split(':');
            _host = addrInfo[0];
            _port = ushort.Parse(addrInfo[1]);
        }
        public string GetHost() => _host;
        public ushort GetPort() => _port;
        public override string ToString()
            => string.Format("{0}:{1}", _host, _port.ToString());
    }
}
