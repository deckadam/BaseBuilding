using System;
using System.Collections.Generic;
using Deck.Data.Component;
using Deck.MVC;
using Deck.Player;
using Deck.Data.Damage;
using Deck.Utility.Logger;
using UnityEngine;

namespace Deck.Components
{
    public class DeckHealthComponent : IDeckComponent
    {
        private DeckHealthData _healthData;
        private DeckAgent _agent;
        private int _currentHealth;
        private int _healthLimit;
        private bool _isDataSetted;
        private Action OnHealthChanged;

        private DeckMVCController<DeckHealthComponent, IEnumerable<DeckHealthComponent>> _healthController;

        public void SetData(DeckHealthData healthData)
        {
            _isDataSetted = true;
            _healthData = healthData;
            _healthLimit = _healthData.health;
        }

        public void Initialize(DeckAgent deckCoreAgent)
        {
            if (!_isDataSetted)
            {
                throw new Exception("Initializing before data set");
            }

            _currentHealth = _healthData.health;
            _agent = deckCoreAgent;

            _healthController = DeckMVC<DeckHealthComponent, IEnumerable<DeckHealthComponent>>.GetController();
            _healthController.GetModel().AddData(this);
        }

        public void DeInitialize()
        {
            _healthController.GetModel().RemoveData(this);
        }

        public void ChangeHealth(DeckDamageData damageData, bool canKill = true)
        {
            _currentHealth -= damageData.damageAmount;
            OnHealthChanged();
            if (canKill && _currentHealth <= 0)
            {
                DeckLogger.Component("Health is zero");
                _currentHealth = 0;
                _agent.Die();
            }
        }

        public int GetHealth()
        {
            return _currentHealth;
        }

        public float GetHealthRatio()
        {
            return (float) _currentHealth / _healthLimit;
        }

        public void SetHealth(int newValue, bool limit = true)
        {
            _currentHealth = newValue;

            if (limit)
            {
                _currentHealth = Mathf.Clamp(_currentHealth, 0, _healthLimit);
            }

            OnHealthChanged();

            DeckLogger.Component("Setting health to  " + newValue, _agent.gameObject);
        }

        public void Register(Action listener)
        {
            OnHealthChanged += listener;
        }

        public void Unregister(Action listener)
        {
            OnHealthChanged -= listener;
        }

        public DeckAgent GetAgent()
        {
            return _agent;
        }
    }
}