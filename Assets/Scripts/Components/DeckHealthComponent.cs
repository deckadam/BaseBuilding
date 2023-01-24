using System;
using System.Collections.Generic;
using Deck.Component;
using Deck.Data.Component;
using Deck.Data.Damage;
using Deck.MVC;
using Deck.Utility.Logger;
using UnityEngine;

namespace Deck.Components
{
    public class DeckHealthComponent : IDeckComponent
    {
        private Action OnHealthChanged;

        private DeckDataHealth _dataHealth;
        private DeckComponentHolder _agent;

        private int _currentHealth;
        private int _limitHealth;
        private bool _isInitialized;

        private DeckMVCController<DeckHealthComponent, IEnumerable<DeckHealthComponent>> _healthController;

        public void Initialize(DeckComponentHolder agent)
        {
            if (_isInitialized)
            {
                return;
            }

            _isInitialized = true;

            _dataHealth = agent.GetData<DeckDataHealth>();
            _limitHealth = _dataHealth.Health;

            _currentHealth = _dataHealth.Health;
            _agent = agent;

            _healthController = DeckMVC<DeckHealthComponent, IEnumerable<DeckHealthComponent>>.GetController();
            _healthController.GetModel().AddData(this);
        }

        public void DeInitialize()
        {
            if (!_isInitialized)
            {
                return;
            }

            _isInitialized = false;

            _healthController.GetModel().RemoveData(this);
        }

        public void ChangeHealth(DeckDataDamage dataDamage, bool canKill = true)
        {
            _currentHealth -= dataDamage.damageAmount;
            OnHealthChanged();
            if (canKill && _currentHealth <= 0)
            {
                DeckLogger.Component("Health is zero");
                _currentHealth = 0;
                _agent.RequestDeath();
            }
        }

        public int GetHealth()
        {
            return _currentHealth;
        }

        public float GetHealthRatio()
        {
            return (float) _currentHealth / _limitHealth;
        }

        public void SetHealth(int newValue, bool limit = true)
        {
            _currentHealth = newValue;

            if (limit)
            {
                _currentHealth = Mathf.Clamp(_currentHealth, 0, _limitHealth);
            }

            OnHealthChanged();

            DeckLogger.Component("Setting health to  " + newValue);
        }

        public void Register(Action listener)
        {
            OnHealthChanged += listener;
        }

        public void Unregister(Action listener)
        {
            OnHealthChanged -= listener;
        }

        public DeckComponentHolder GetComponentOwner()
        {
            return _agent;
        }

        public object GetData()
        {
            return new DeckHealthComponentData
            {
                currentHealth = _currentHealth,
                limitHealth = _limitHealth
            };
        }

        public void LoadData(string value)
        {
            var data = JsonUtility.FromJson<DeckHealthComponentData>(value);
            _currentHealth = data.currentHealth;
            _limitHealth = data.limitHealth;
            OnHealthChanged();
        }

        public void Tick()
        {
        }


        [Serializable]
        public class DeckHealthComponentData
        {
            public int currentHealth;
            public int limitHealth;
        }
    }
}