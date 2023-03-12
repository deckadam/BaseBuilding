using System;
using System.Collections.Generic;
using System.Linq;
using Components;
using Data.Component;
using Deck.Components;
using Deck.Components.Operations;
using Deck.Save.Data;
using UnityEngine;

namespace Deck.Agent
{
    public abstract class DeckAgent : MonoBehaviour
    {
        public DeckComponent[] components { get; private set; }
        public float defaultSize = 1f;
        [SerializeField] private float _activeSize = -1f;
        private Dictionary<Type, DeckComponentData> _datas;
        private bool _hasSetComponents;
        private DeckAgentCommandProcessor _commandProcessor;
        protected string id;

        public void StartWithClearData()
        {
            Initialize();
            id = GetType().ToString().Split('.')[^1];
        }

        protected void SetComponents(params DeckComponent[] components)
        {
            _hasSetComponents = true;
            this.components = components;
        }

        public T GetDeckComponent<T>() where T : DeckComponent
        {
            return (T)components.FirstOrDefault(item => item.GetType() == typeof(T));
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

            _commandProcessor = new DeckAgentCommandProcessor();
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

            _commandProcessor.StopExecutions();
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
                if (temp == null)
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

        public void LoadData(DeckComponentHolderSaveData data)
        {
            Initialize();
            id = data.id;
            foreach (var saveData in data.componentDatas)
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

        public T GetData<T>() where T : DeckComponentData
        {
            if (_datas.TryGetValue(typeof(T), out var result))
            {
                return (T)result;
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

        public string GetId()
        {
            return id;
        }

        public void AddCommand(DeckCommand command)
        {
            _commandProcessor.AddCommand(command);
        }

        public virtual float GetSize() => Math.Abs(_activeSize + 1) < 0.001f ? defaultSize : _activeSize;
        public abstract void RequestDeath();
    }
}