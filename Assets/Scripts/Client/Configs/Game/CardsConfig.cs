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

        [Serializable]
        private struct NumberOfColorCardsByPlayerCountData
        {
            public string Comment;
            
            [Min(2)]
            public int PlayerCount;
            
            [Min(0)]
            public int NumberOfColorCards;
        }
        
        [Header("Space Cards")]
        [SerializeField, Min(0)]
        private int _numberOfConversationsSpaceCards;

        [SerializeField, Min(0)] 
        private int _playerStartingNumberOfSpaceCards;
        
        [SerializeField]
        private DamageSpaceCardData[] _damageSpaceCards = null!;
        
        [Header("Destiny Cards")]
        [SerializeField]
        private NumberOfColorCardsByPlayerCountData[]  _numberOfColorCardsByPlayerCountData = null!;
        
        [SerializeField]
        private int _numberOfJokersDestiny;

        public CardsData BuildData()
        {
            var decks = CreateDecks();
            var destinyCardsGenerationData = CreateDestinyCardsGenerationData();

            return new CardsData(decks, _playerStartingNumberOfSpaceCards, destinyCardsGenerationData);
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

        private DestinyCardsGenerationData CreateDestinyCardsGenerationData()
        {
            var dependence = _numberOfColorCardsByPlayerCountData
                .ToDictionary(item => 
                    item.PlayerCount, 
                    item => item.NumberOfColorCards);

            return new DestinyCardsGenerationData(dependence, _numberOfJokersDestiny);
        }
    }
}