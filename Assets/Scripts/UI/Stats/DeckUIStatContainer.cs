using System.Collections.Generic;
using Deck.Utility.Poolable;
using Zenject;

namespace Deck.UI.Stats
{
    public class DeckUIStatContainer : DeckPoolable
    {
        private DeckUIStatElement.Factory _statElementFactory;
        private List<DeckUIStatElement> _activeElements;
        
        
        [Inject]
        private void Inject(DeckUIStatElement.Factory statElementFactory)
        {
            _statElementFactory = statElementFactory;
            _activeElements = new List<DeckUIStatElement>();
        }

        public void SetStats(DeckStat[] stats)
        {
            foreach (var deckUIStatElement in _activeElements)
            {
                deckUIStatElement.Despawn();
            }
            
            _activeElements.Clear();

            foreach (var stat in stats)
            {
                var statElement = _statElementFactory.Create();
                statElement.SetStat(stat);
                statElement.transform.SetParent(transform);
                _activeElements.Add(statElement);
            }
        }

        protected override void Despawned()
        {
            foreach (var deckUIStatElement in _activeElements)
            {
                deckUIStatElement.Despawn();
            }
            
            _activeElements.Clear();
        }

        public class Factory : PlaceholderFactory<DeckUIStatContainer>
        {
        }
    }
}