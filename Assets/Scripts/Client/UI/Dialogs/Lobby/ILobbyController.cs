namespace Client.UI.Dialogs.Lobby
{
    public interface ILobbyController
    {
        bool IsOwnerLobby { get; }

        void StartHost();
        
        void StartClient();
    }
}