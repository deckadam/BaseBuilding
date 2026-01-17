using System.Collections.Generic;
using Base;
using UnityEngine;
using Zenject;

namespace UI.Stats
{
    public class DeckStatsPopUp : DeckPopUpBase
    {
        [SerializeField] private Transform _statsContainer;
        [SerializeField] private DeckUIStatContainer _statContainerPrefab;

        private Dictionary<DeckComponent, DeckUIStatContainer> _activeStats;
        private DeckAgent _agent;

        [Inject]
        private void Inject()
        {
            _activeStats = new Dictionary<DeckComponent, DeckUIStatContainer>();
        }

        public void ShowStats(DeckAgent agent)
        {
            _agent = agent;
            var stats = _agent.GetStats();

            foreach (var stat in stats)
            {
                if (!stat.IsValid || stat.Stats == null || stat.Stats.Length == 0)
                {
                    continue;
                }

                var statContainer = InstanceProvider.RentUIElement<DeckUIStatContainer>();
                statContainer.SetStats(stat.Stats);
                statContainer.transform.SetParent(_statsContainer);
                _activeStats[stat.Component] = statContainer;

                stat.Component.OnStatsChanged += OnStatsChanged;
            }
        }

        protected override void InternalOnDeSpawned()
        {
            foreach (var stat in _activeStats)
            {
                stat.Key.OnStatsChanged -= OnStatsChanged;
            }

            foreach (var statContainer in _activeStats)
            {
                InstanceProvider.ReturnUIElement(statContainer.Value);
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
                var statContainer = InstanceProvider.RentUIElement<DeckUIStatContainer>();
                statContainer.SetStats(statGroup.Stats);
                statContainer.transform.SetParent(_statsContainer);
                _activeStats[statGroup.Component] = statContainer;
            }
        }
    }
}