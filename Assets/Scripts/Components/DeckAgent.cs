using System;
using System.Collections.Generic;
using System.Linq;
using Data.Component;
using Deck.Components;
using Deck.Components.Operations;
using UnityEngine;

namespace Deck.Component
{
    [Serializable]
    public abstract class DeckAgent : MonoBehaviour
    {
        public float defaultSize = 1f;

        [SerializeReference] public DeckComponent[] components;
        private Dictionary<Type, DeckComponentData> _datas;
        private Dictionary<DeckCommandType, List<Action<DeckCommand>>> _commandListeners;
        private bool _hasSetComponents;
        [SerializeField] private float _activeSize = -1f;

        protected void SetComponents(params DeckComponent[] components)
        {
            _hasSetComponents = true;
            this.components = components;
            GenerateCommandListeners(components);
        }

        private void GenerateCommandListeners(DeckComponent[] components)
        {
            _commandListeners = new Dictionary<DeckCommandType, List<Action<DeckCommand>>>();
            for (var i = 0; i < components.Length; i++)
            {
                var listener = components[i].GetSupportedCommandTypes();
                for (var j = 0; j < listener.Length; j++)
                {
                    if (_commandListeners.ContainsKey(listener[j].commandType))
                    {
                        _commandListeners[listener[j].commandType].Add(listener[j].listener);
                    }
                    else
                    {
                        _commandListeners[listener[j].commandType] = new List<Action<DeckCommand>>();
                        _commandListeners[listener[j].commandType].Add(listener[j].listener);
                    }
                }
            }
        }

        public T GetDeckComponent<T>() where T : DeckComponent
        {
            return (T) components.First(item => item.GetType() == typeof(T));
        }

        public T[] GetDeckComponents<T>() where T : DeckComponent
        {
            return components as T[];
        }

        protected void Initialize()
        {
            if (!_hasSetComponents)
            {
                throw new Exception("Components hasn't been set");
            }

            foreach (var deckComponent in components)
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

            foreach (var deckComponent in components)
            {
                deckComponent.DeInitialize();
            }
        }

        private void OnDestroy()
        {
            DeInitialize();
        }

        private void Update()
        {
            if (!_hasSetComponents)
            {
                throw new Exception("Components hasn't been set");
            }

            foreach (var deckComponent in components)
            {
                deckComponent.Tick();
            }
        }

        public List<DeckComponentSaveData> GetSaveData()
        {
            var result = new List<DeckComponentSaveData>();
            foreach (var deckComponent in components)
            {
                var temp = deckComponent.GetData();
                if (temp != null)
                {
                    continue;
                }

                var serializedValue = JsonUtility.ToJson(temp);
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
                foreach (var component in components)
                {
                    if (component.GetType().ToString() == saveData.id)
                    {
                        component.LoadData(saveData.data);
                    }
                }
            }
        }

        public void Possess()
        {
            DispatchCommand(new DeckCommandPossess());
        }

        public void Release()
        {
            DispatchCommand(new DeckCommandRelease());
        }

        public void DispatchCommand(DeckCommand command)
        {
            var commandType = command.commandType;
            if (_commandListeners.TryGetValue(commandType, out var listeners))
            {
                foreach (var deckComponent in listeners)
                {
                    deckComponent(command);
                }
            }
        }

        public T GetData<T>() where T : DeckComponentData
        {
            if (_datas.TryGetValue(typeof(T), out var result))
            {
                return (T) result;
            }

            return null;
        }

        protected void SetComponentDatas(DeckComponentData[] data)
        {
            _datas = new Dictionary<Type, DeckComponentData>();
            foreach (var temp in data)
            {
                _datas[temp.GetType()] = Instantiate(temp);
            }
        }

        public virtual float GetSize() => Math.Abs(_activeSize + 1) < 0.001f ? defaultSize : _activeSize;
        public abstract void RequestDeath();
        public abstract string GetName();
    }
}