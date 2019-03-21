namespace RakDotNet
{
    public struct UpnpPort
    {
        public ushort PrivatePort { get; }
        public ushort PublicPort { get; }
        public int LifeTime { get; }

        public UpnpPort(ushort privatePort, ushort publicPort, int lifeTime)
        {
            PrivatePort = privatePort;
            PublicPort = publicPort;
            LifeTime = lifeTime;
        }
    }
}