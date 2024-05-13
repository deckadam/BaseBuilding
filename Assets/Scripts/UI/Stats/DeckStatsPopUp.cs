using System.Collections.Generic;
using Deck.Agent;
using Deck.Commands;
using UnityEngine;
using Zenject;

namespace Deck.UI.Stats
{
    public class DeckStatsPopUp : DeckPopUpBase
    {
        [SerializeField] private Transform _statsContainer;
        private DeckUIStatContainer.Factory _statContainerFactory;
        private Dictionary<DeckComponent, DeckUIStatContainer> _activeStats;
        private DeckAgent _agent;

        [Inject]
        private void Inject(DeckUIStatContainer.Factory statContainerFactory)
        {
            _statContainerFactory = statContainerFactory;

            _activeStats = new Dictionary<DeckComponent, DeckUIStatContainer>();
        }

        public void ShowStats(DeckAgent agent)
        {
            _agent = agent;
            var stats = _agent.GetStats();

            foreach (var stat in stats)
            {
                if (stat.Stats == null || stat.Stats.Length == 0)
                {
                    continue;
                }

                var statContainer = _statContainerFactory.Create();
                statContainer.SetStats(stat.Stats);
                statContainer.transform.SetParent(_statsContainer);
                _activeStats[stat.Component] = statContainer;

                stat.Component.OnStatsChanged += OnStatsChanged;
            }
        }

        protected override void Despawned()
        {
            foreach (var stat in _activeStats)
            {
                stat.Key.OnStatsChanged -= OnStatsChanged;
            }

            _activeStats.Clear();
        }

        private void OnStatsChanged(DeckStatGroup statGroup)
        {
            if (statGroup.Stats == null || statGroup.Stats.Length == 0)
            {
                return;
            }

            if (_activeStats.ContainsKey(statGroup.Component))
            {
                _activeStats[statGroup.Component].SetStats(statGroup.Stats);
            }
            else
            {
                var statContainer = _statContainerFactory.Create();
                statContainer.SetStats(statGroup.Stats);
                statContainer.transform.SetParent(_statsContainer);
                _activeStats[statGroup.Component] = statContainer;
            }
        }

        public class Factory : PlaceholderFactory<DeckStatsPopUp>
        {
        }
    }
}