using System;
using System.Collections.Generic;
using System.Linq;
using Deck.Components;
using UnityEngine;

namespace Deck.Component
{
    public abstract class DeckComponentHolder : MonoBehaviour
    {
        private IDeckComponent[] _components;
        private bool _hasSetComponents;

        public float defaultSize = 1f;
        protected float _activeSize = -1f;

        protected void SetComponents(params IDeckComponent[] components)
        {
            _hasSetComponents = true;
            _components = components;
        }

        public T GetDeckComponent<T>() where T : IDeckComponent
        {
            return (T) _components.First(item => item.GetType() == typeof(T));
        }

        public void Initialize()
        {
            if (!_hasSetComponents)
            {
                throw new Exception("Components hasn't been set");
            }

            foreach (var deckComponent in _components)
            {
                deckComponent.Initialize(this);
            }
        }

        protected void DeInitialize()
        {
            if (!_hasSetComponents)
            {
                throw new Exception("Components hasn't been set");
            }

            foreach (var deckComponent in _components)
            {
                deckComponent.DeInitialize();
            }
        }

        protected void Tick()
        {
            if (!_hasSetComponents)
            {
                throw new Exception("Components hasn't been set");
            }

            foreach (var deckComponent in _components)
            {
                deckComponent.Tick();
            }
        }

        public List<DeckComponentSaveData> GetSaveData()
        {
            var result = new List<DeckComponentSaveData>();
            foreach (var deckComponent in _components)
            {
                var serializedValue = JsonUtility.ToJson(deckComponent.GetData());
                result.Add(new DeckComponentSaveData
                {
                    id = deckComponent.GetType().ToString(),
                    data = serializedValue
                });
            }

            return result;
        }

        protected void LoadComponentData(List<DeckComponentSaveData> data)
        {
            foreach (var saveData in data)
            {
                foreach (var component in _components)
                {
                    if (component.GetType().ToString() == saveData.id)
                    {
                        component.LoadData(saveData.data);
                    }
                }
            }
        }

        public virtual float GetSize() => Math.Abs(_activeSize + 1) < 0.001f ? defaultSize : _activeSize;
        public abstract void OnPossessionStart();
        public abstract void OnPossessionEnd();
        public abstract T GetData<T>();
        public abstract void RequestDeath();
        public abstract string GetName();
    }
}