namespace RakDotNet.Utils
{
    public struct UPnPSettings
    {
        public ushort PrivatePort { get; }
        public ushort PublicPort { get; }
        public int LifeTime { get; }

        public UPnPSettings(ushort privatePort, ushort publicPort, int lifeTime)
        {
            PrivatePort = privatePort;
            PublicPort = publicPort;
            LifeTime = lifeTime;
        }
    }
}