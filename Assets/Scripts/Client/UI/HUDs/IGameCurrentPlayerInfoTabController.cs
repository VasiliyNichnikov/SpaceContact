using System;
using Client.UI.HUDs.ViewModels;

namespace Client.UI.HUDs
{
    public interface IGameCurrentPlayerInfoTabController
    {
        event Action? Changed;
        
        GamePlayerInfoTabType ActiveTab { get; }
    }
}