using System;

namespace Core.Game.Actions
{
    /// <summary>
    /// A client action that the server may also execute automatically for the current aggressor.
    /// </summary>
    [Serializable]
    public abstract class AutomaticClientGameAction : ClientGameAction
    {
    }
}
