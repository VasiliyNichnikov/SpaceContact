namespace Core
{
    public class CoreNetworkContext
    {
        public bool IsServer { get; private set; }
        
        public bool IsOwner { get; private set; }

        public CoreNetworkContext SetThisServer()
        {
            IsServer = true;

            return this;
        }

        public CoreNetworkContext SetThisOwner()
        {
            IsOwner = true;

            return this;
        }
    }
}