using System;
using System.Collections.Generic;
using System.Linq;
using Data.Component;
using Deck.Components;
using Deck.Save.Data;
using UnityEngine;
using Utility.Enums;

namespace Deck.Agent
{
    public abstract class DeckAgent : MonoBehaviour
    {
        [SerializeField] private float activeSize = -1f;
        [SerializeField] private DeckAgentShape shape;
        [SerializeField] private bool willSave;
        public event Action<float> OnAgentSizeChanged;
        public DeckComponent[] components { get; private set; }
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
            if (activeSize < 0f)
            {
                throw new Exception("Size value can not be lower than zero");
            }

            _commandProcessor = new DeckAgentCommandProcessor();

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

            if (_commandProcessor != null)
            {
                _commandProcessor.StopExecutions();
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

        protected void SetSize(float newSize)
        {
            activeSize = newSize;
            OnAgentSizeChanged?.Invoke(activeSize);
        }

        public float GetSize() => activeSize;
        public DeckAgentShape GetShape() => shape;
        public abstract void RequestDeath();
        public bool WillSave() => willSave;
    }
}