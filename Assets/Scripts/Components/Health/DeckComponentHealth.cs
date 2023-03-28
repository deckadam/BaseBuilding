using System;
using System.Collections.Generic;
using Deck.Data.Component;
using Deck.MVC;
using Deck.Utility.Health;
using Deck.Utility.Logger;
using UnityEngine;
using Zenject;

namespace Deck.Components
{
    [Serializable]
    public class DeckComponentHealth : DeckComponent
    {
        private DeckMVCController<DeckComponentHealth, IEnumerable<DeckComponentHealth>> _healthController;
        private DeckHealthBar.Factory _healthBarFactory;
        private DeckDataHealth _dataHealth;
        private DeckHealthBar _healthBar;
        private bool _hasHealthBar;
        private int _currentHealth;
        private int _limitHealth;
        public event Action OnDamageTaken;

        [Inject]
        private void Inject(DeckHealthBar.Factory healthBarFactory)
        {
            _healthBarFactory = healthBarFactory;
        }

        protected override void InternalPreInitialize()
        {
            _dataHealth = holder.GetData<DeckDataHealth>();
            _limitHealth = _dataHealth.Health;

            _currentHealth = _dataHealth.Health;

            _healthController = DeckMVC<DeckComponentHealth, IEnumerable<DeckComponentHealth>>.GetController();
            _healthController.GetModel().AddData(this);
        }

        public override void DeInitialize()
        {
            _healthController.GetModel().RemoveData(this);
        }

        public void ReduceHealth(int amount, bool canKill = true)
        {
            _currentHealth -= amount;
            OnDamageTaken?.Invoke();
            if (_currentHealth <= 0)
            {
                if (canKill)
                {
                    DeckLogger.Component("Requesting death on " + holder.GetId());
                    ReleaseHealthBar();
                    _currentHealth = 0;
                    holder.RequestDeath();
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
            if (!_hasHealthBar)
            {
                _healthBar = _healthBarFactory.Create();
                _healthBar.SetPositionOffset(Vector3.up * 2f);
                _healthBar.SetTarget(GetComponentHolder().transform);
                _hasHealthBar = true;
            }
        }

        private void ReleaseHealthBar()
        {
            if (_hasHealthBar)
            {
                _healthBar.Despawn();
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
            _currentHealth = newValue;

            if (limit)
            {
                _currentHealth = Mathf.Clamp(_currentHealth, 0, _limitHealth);
            }

            ReduceHealth(0);

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
            var data = JsonUtility.FromJson<DeckHealthComponentData>(value);
            _currentHealth = data.currentHealth;
            _limitHealth = data.limitHealth;
            ReduceHealth(0);
        }

        [Serializable]
        public class DeckHealthComponentData
        {
            public int currentHealth;
            public int limitHealth;
        }
    }
}