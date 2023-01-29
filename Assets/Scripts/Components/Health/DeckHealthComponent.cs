using System;
using System.Collections.Generic;
using Deck.Components.Operations;
using Deck.Data.Component;
using Deck.MVC;
using Deck.Utility.Logger;
using UnityEngine;

namespace Deck.Components
{
    [Serializable]
    public class DeckHealthComponent : DeckComponent
    {
        private Action OnHealthChanged;

        private DeckDataHealth _dataHealth;

        private int _currentHealth;
        private int _limitHealth;
        private bool _isInitialized;

        private DeckMVCController<DeckHealthComponent, IEnumerable<DeckHealthComponent>> _healthController;

        protected override void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            _isInitialized = true;

            _dataHealth = holder.GetData<DeckDataHealth>();
            _limitHealth = _dataHealth.Health;

            _currentHealth = _dataHealth.Health;

            _healthController = DeckMVC<DeckHealthComponent, IEnumerable<DeckHealthComponent>>.GetController();
            _healthController.GetModel().AddData(this);
        }

        public override void DeInitialize()
        {
            if (!_isInitialized)
            {
                return;
            }

            _isInitialized = false;

            _healthController.GetModel().RemoveData(this);
        }

        private void ChangeHealth(DeckCommand dataCommandDamage)
        {
            var convertedCommand = dataCommandDamage as DeckCommandDamage;
            _currentHealth -= convertedCommand.damageData.damageAmount;
            OnHealthChanged();
            if (_currentHealth <= 0)
            {
                if (convertedCommand.canKill)
                {
                    DeckLogger.Component("Health is zero");
                    _currentHealth = 0;
                    holder.RequestDeath();
                }
                else
                {
                    _currentHealth = 1;
                }
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

        public override DeckCommandListener[] GetSupportedCommandTypes()
        {
            return new[] {DeckCommandListener.Create(DeckCommandType.TakeDamage, ChangeHealth)};
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
            var data = JsonUtility.FromJson<DeckHealthComponentData>(value);
            _currentHealth = data.currentHealth;
            _limitHealth = data.limitHealth;
            OnHealthChanged();
        }

        [Serializable]
        public class DeckHealthComponentData
        {
            public int currentHealth;
            public int limitHealth;
        }
    }
}