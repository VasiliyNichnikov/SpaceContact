using System;
using System.Collections.Generic;
using System.Linq;
using Core.EngineData;
using Core.Lobby.Dto;
using Logs;

namespace Core.User
{
    public class UsersColorController : IUsersColorProvider
    {
        private class ColorState
        {
            public ulong? AttachedUserId;
            
            public Color Color;
        }
        
        private readonly Dictionary<int, ColorState> _colorStateById;
        
        public UsersColorController(LobbySettingsData data)
        {
            _colorStateById = InitColorStateById(data);
        }
        
        public event Action? ColorsChanged;

        public int GetFreeColor()
        {
            var kvp = _colorStateById.First(kvp => 
                kvp.Value.AttachedUserId == null);
            var colorId = kvp.Key;
            
            return colorId;
        }
        
        public IReadOnlyCollection<int> AllColorIds => 
            _colorStateById.Keys;

        public bool IsColorAvailableForSelection(int colorId)
        {
            return _colorStateById.ContainsKey(colorId) &&
                   _colorStateById[colorId].AttachedUserId == null;
        }

        public Color GetColor(int colorId) => 
            _colorStateById[colorId].Color;

        public int? GetColorByUserId(ulong userId)
        {
            var kvp = _colorStateById.FirstOrDefault(kvp => kvp.Value.AttachedUserId == userId);

            if (kvp.Value == null)
            {
                return null;
            }
            
            return kvp.Key;
        }
        
        public void AttachUserToColor(ulong attachedUserId, int colorId)
        {
            if (!_colorStateById.TryGetValue(colorId, out var colorState))
            {
                Logger.Error($"UsersColorProvider.AssignUserToColor: color with id {colorId} not found.");
                
                return;
            }

            colorState.AttachedUserId = attachedUserId;
            
            ColorsChanged?.Invoke();
        }

        public void DetachUserFromColor(ulong attachedUserId)
        {
            var colorId = GetColorByUserId(attachedUserId);

            if (colorId == null)
            {
                Logger.Error($"UsersColorProvider.DetachUserFromColor: color for user {attachedUserId} not found.");
                return;
            }
            
            _colorStateById.Remove(colorId.Value);
        }

        public bool TryChangeColor(ulong userId, int newColorId)
        {
            var previousColorId = GetColorByUserId(userId);

            if (previousColorId == null)
            {
                Logger.Error($"UsersColorProvider.ChangeColor: colorId for user with id {userId} not found.");
                
                return false;
            }
            
            if (!_colorStateById.TryGetValue(previousColorId.Value, out var colorState))
            {
                Logger.Error($"UsersColorProvider.ChangeColor: color with id {newColorId} not found.");
                
                return false;
            }
            
            colorState.AttachedUserId = null;
            _colorStateById[newColorId].AttachedUserId = userId;
            
            ColorsChanged?.Invoke();
            
            return true;
        }

        private static Dictionary<int, ColorState> InitColorStateById(LobbySettingsData data)
        {
            var id = 0;
            var result = new Dictionary<int, ColorState>();
            
            foreach (var color in data.AllAvailablePlayerColors)
            {
                var state = new ColorState
                {
                    Color = color,
                    AttachedUserId = null
                };
                
                result.Add(id++, state);
            }
            
            return result;
        }
    }
}