using System;
using System.Collections.Generic;
using System.Linq;
using Core.Game.Dto.Rules.Cards;
using UnityEngine;

namespace Client.Configs.Game
{
    [CreateAssetMenu(fileName = "CardsConfig", menuName = "Configs/Game/CardsConfig", order = 0)]
    public class CardsConfig : ScriptableObject
    {
        [Serializable]
        private struct DamageSpaceCardData
        {
            public string Comment;
            
            [Min(0)]
            public int Count;

            public int DamageCount;
        }
        
        [SerializeField, Min(0)]
        private int _numberOfConversationsSpaceCards;

        [SerializeField, Min(0)] 
        private int _playerStartingNumberOfSpaceCards;
        
        [SerializeField]
        private DamageSpaceCardData[] _damageSpaceCards = null!;

        public CardsData BuildData()
        {
            var decks = CreateDecks();

            return new CardsData(decks, _playerStartingNumberOfSpaceCards);
        }

        private DecksData CreateDecks()
        {
            var damages = CreateDamages();

            return new DecksData(damages,
                Array.Empty<SpaceCardArtifactData>(),
                _playerStartingNumberOfSpaceCards);
        }

        private IReadOnlyCollection<SpaceCardDamageData> CreateDamages()
        {
            return _damageSpaceCards
                .Select(item => new SpaceCardDamageData(item.Count, item.DamageCount))
                .ToList();
        }
    }
}