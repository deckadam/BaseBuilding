using System;
using System.Collections.Generic;
using Deck.Agent;
using Deck.Data.Buildable;
using Zenject;

namespace Deck.Services.Building
{
    public class DeckBuildingService : DeckServiceBase
    {
        private DeckBuildable[] _buildables;
        private DiContainer _container;
        private Dictionary<string, DeckBuildable> _buildableDictionary;

        [Inject]
        private void Inject(DeckBuildable[] buildables, DiContainer container)
        {
            _container = container;
            _buildables = buildables;
            _buildableDictionary = new Dictionary<string, DeckBuildable>();
            foreach (var deckBuildable in _buildables)
            {
                _buildableDictionary[deckBuildable.GetName()] = deckBuildable;
            }
        }

        public T Build<T>(DeckBuildable buildable) where T : DeckAgent
        {
            return _container.InstantiatePrefab(buildable.GetAgent().gameObject).GetComponent<T>();
        }

        public DeckBuildable GetBuildable(string name)
        {
            if (_buildableDictionary.TryGetValue(name, out var match))
            {
                return match;
            }

            throw new Exception("Buildable not found");
        }
    }
}