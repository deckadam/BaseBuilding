using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Deck.Components;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;
using SerializationUtility = Sirenix.Serialization.SerializationUtility;

namespace Deck.Component
{
    public abstract class DeckComponentHolder : MonoBehaviour
    {
        private IDeckComponent[] _components;
        private bool _hasSetComponents;

        protected void SetComponents(params IDeckComponent[] components)
        {
            _hasSetComponents = true;
            _components = components;
        }

        protected T GetDeckComponent<T>() where T : IDeckComponent
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

        public abstract T GetData<T>();
        public abstract void RequestDeath();
        public abstract string GetName();
    }
}