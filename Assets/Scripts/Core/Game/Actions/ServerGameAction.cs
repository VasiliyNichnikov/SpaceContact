using System;

namespace Core.Game.Actions
{
    /// <summary>
    /// An action that only the server may initiate, such as changing a phase or starting a timer.
    /// This type restricts executor inputs and prevents these actions from being accepted in client requests.
    /// The name identifies the request source: ClientGameAction also executes exclusively on the server.
    /// </summary>
    [Serializable]
    public abstract class ServerGameAction : BaseGameAction
    {
    }
}
