using System;

namespace Core.Game.Actions
{
    /// <summary>
    /// An action that a client may request the server to execute.
    /// Concrete actions still validate the actor and game rules on the server.
    /// </summary>
    [Serializable]
    public abstract class ClientGameAction : BaseGameAction
    {
        // nothing
    }
}
