using System;
using System.Collections.Generic;
using Core.EngineData;

namespace Core.User
{
    public interface IUsersColorProvider
    {
        event Action? ColorsChanged;

        IReadOnlyCollection<int> AllColorIds { get; }
        
        bool IsColorAvailableForSelection(int colorId);
        
        Color GetColor(int colorId);
    }
}