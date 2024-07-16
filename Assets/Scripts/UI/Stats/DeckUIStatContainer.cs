using System;
using System.Collections.Generic;
using Deck.UI.Pool;
using Deck.Utility.Poolable;
using UnityEngine;
using Zenject;

namespace Deck.UI.Stats
{
    public class DeckUIStatContainer : DeckUIElement
    {
        [SerializeField] private DeckUIStatElement _statElementPrefab;
        private DeckUIPool _uiPool;
        private List<DeckUIStatElement> _activeElements;


        [Inject]
        private void Inject(DeckUIPool uiPool)
        {
            _uiPool = uiPool;
            _activeElements = new List<DeckUIStatElement>();
        }

        public void SetStats(DeckStat[] stats)
        {
            ClearStatElements();

            foreach (var stat in stats)
            {
                var statElement = _uiPool.Rent<DeckUIStatElement>(_statElementPrefab.PrefabId);
                statElement.SetStat(stat);
                statElement.transform.SetParent(transform);
                _activeElements.Add(statElement);
            }
        }

        private void ClearStatElements()
        {
            _uiPool.Return(_activeElements);
            _activeElements.Clear();
        }

        protected override void OnDespawned()
        {
            ClearStatElements();
        }
    }
}