using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using Data.Component;
using Instancing;
using Sirenix.OdinInspector;
using Systems.SystemSave;
using UI.Health;
using UI.Stats;
using UnityEngine;
using Utility;
using Utility.MVC;
using Zenject;

namespace Components.Health
{
    [Serializable]
    public class DeckComponentHealth : DeckComponent
    {
        private const string HEALTH_STAT_DESCRIPTION = "Current health of the agent";
        private const string HEALTH_STAT_NAME = "Health";

        [SerializeField] private DeckDataHealth dataHealth;

        [ReadOnly, SerializeField] private int _currentHealth;

        private DeckMVCController<DeckComponentHealth, IEnumerable<DeckComponentHealth>> _healthController;
        private DeckHealthBar _healthBar;
        private bool _hasHealthBar;
        private int _limitHealth;
        private DeckInstanceProvider _instanceProvider;
        public bool IsDead => _currentHealth <= 0;

        public event Action<DeckAgent> OnDamageTaken;

        [Inject]
        private void Inject(DeckInstanceProvider instanceProvider)
        {
            _instanceProvider = instanceProvider;
        }

        protected override void InternalPreInitialize()
        {
            _limitHealth = dataHealth.Health;
            _currentHealth = dataHealth.Health;

            _healthController = DeckMVC<DeckComponentHealth, IEnumerable<DeckComponentHealth>>.GetController();
            _healthController.GetModel().AddData(this);
        }

        public bool CanBeDamagedByAnyOfTags(List<DeckActionTag> tags)
        {
            return tags.Select(t => dataHealth.HasTag(t)).Any(result => result);
        }

        public override void DeInitialize()
        {
            _healthController.GetModel().RemoveData(this);
        }

        public void ReduceHealth(DeckAgent damageDealer, int amount, bool canKill = true)
        {
            _currentHealth -= amount;
            OnDamageTaken?.Invoke(damageDealer);
            OnStatsChanged?.Invoke(GetStatGroup());

            if (_currentHealth <= 0)
            {
                if (canKill)
                {
                    DeckLogger.Component("Requesting death on " + agent.UniqueId.Id);
                    ReleaseHealthBar();
                    _currentHealth = 0;
                    agent.RequestDestroy();
                    return;
                }

                _currentHealth = 1;
            }

            if (_currentHealth < _limitHealth)
            {
                GetHealthBar();
                _healthBar.OnDataChanged(_currentHealth, (float)_currentHealth / _limitHealth);
            }
            else
            {
                ReleaseHealthBar();
            }
        }

        private void GetHealthBar()
        {
            if (_hasHealthBar) return;

            _healthBar = _instanceProvider.RentUIElement<DeckHealthBar>();
            _healthBar.SetPositionOffset(Vector3.up * 2f);
            _healthBar.SetTarget(GetAgent().transform);
            _hasHealthBar = true;
        }

        private void ReleaseHealthBar()
        {
            if (_hasHealthBar)
            {
                _instanceProvider.ReturnUIElement(_healthBar);
                _hasHealthBar = false;
            }
        }

        public int GetHealth()
        {
            return _currentHealth;
        }

        public float GetHealthRatio()
        {
            return (float)_currentHealth / _limitHealth;
        }

        public void SetHealth(int newValue, bool limit = true)
        {
            if (newValue == _currentHealth)
            {
                return;
            }

            _currentHealth = newValue;

            if (limit)
            {
                _currentHealth = Mathf.Clamp(_currentHealth, 0, _limitHealth);
            }

            ReduceHealth(null, 0);

            OnStatsChanged(GetStatGroup());

            DeckLogger.Component("Setting health to  " + newValue);
        }

        public override object GetData()
        {
            return new DeckHealthComponentData
            {
                currentHealth = _currentHealth,
                limitHealth = _limitHealth
            };
        }

        public override void LoadData(string value)
        {
            var data = DeckSaveUtility.GetDeserializedData<DeckHealthComponentData>(value);
            _currentHealth = data.currentHealth;
            _limitHealth = data.limitHealth;
            ReduceHealth(null, 0);
        }

        public List<DeckActionTag> GetDamagingTags()
        {
            return dataHealth.GetTags();
        }

        public override DeckStatGroup GetStatGroup()
        {
            return new DeckStatGroup(new[]
            {
                new DeckStat(HEALTH_STAT_NAME, _currentHealth + " - " + _limitHealth, HEALTH_STAT_DESCRIPTION)
            }, this);
        }

        [Serializable]
        public class DeckHealthComponentData
        {
            public int currentHealth;
            public int limitHealth;
        }
    }
}