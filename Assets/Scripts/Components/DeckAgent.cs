using System;
using System.Collections.Generic;
using System.Linq;
using Data.Component;
using Deck.Components;
using Deck.Events.MapService;
using Deck.Save.Data;
using UnityEngine;
using Utility.Enums;

namespace Deck
{
    [RequireComponent(typeof(Collider))]
    public abstract class DeckAgent : MonoBehaviour
    {
        [SerializeField] private float activeSize = -1f;
        [SerializeField] private DeckAgentShape shape;
        [SerializeField] private bool willSave;
        [SerializeField] private new Collider collider;
        [SerializeField] protected Transform centerPosition;
        [SerializeField] private DeckDataComponent[] data;

        public event Action<float> OnAgentSizeChanged;

        protected DeckComponent[] components { get; private set; }
        protected string id;

        private Dictionary<Type, DeckDataComponent> _datas;
        private bool _hasSetComponents;
        private DeckCommandProcessor _commandProcessor;
        private bool _alreadyDeinitialized;
        private bool _hasBeenInitialized;

        public void StartWithClearData()
        {
            Initialize();
            id = GetType().ToString().Split('.')[^1];
        }

        private void Start()
        {
            Initialize();
            transform.SetParent(DeckServiceMap.GetMap().transform, true);
        }

        private void OnValidate()
        {
            collider = GetComponent<Collider>();
        }

        protected void SetComponents(params DeckComponent[] components)
        {
            _hasSetComponents = true;
            this.components = components;
        }

        public T GetDeckComponent<T>() where T : class
        {
            for (var i = 0; i < components.Length; i++)
            {
                if (components[i] is T component)
                {
                    return component;
                }
            }

            return null;
        }

        public T[] GetDeckComponents<T>() where T : DeckComponent
        {
            return components as T[];
        }

        protected void Initialize()
        {
            if (_hasBeenInitialized)
            {
                return;
            }

            if (activeSize < 0f)
            {
                throw new Exception("Size value can not be lower than zero");
            }

            SetComponentDatas(data);
            _commandProcessor = new DeckCommandProcessor();

            if (!_hasSetComponents)
            {
                throw new Exception("Components hasn't been set");
            }

            _hasBeenInitialized = true;

            foreach (var deckComponent in components)
            {
                deckComponent.PreInitialize(this);
            }

            foreach (var deckComponent in components)
            {
                deckComponent.PostInitialize();
            }
        }

        protected void DeInitialize()
        {
            if (_alreadyDeinitialized)
            {
                return;
            }

            if (!_hasSetComponents)
            {
                throw new Exception("Components hasn't been set");
            }

            _alreadyDeinitialized = true;

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

        public T GetData<T>() where T : DeckDataComponent
        {
            if (_datas.TryGetValue(typeof(T), out var result))
            {
                return (T)result;
            }

            return null;
        }

        private void SetComponentDatas(DeckDataComponent[] data)
        {
            _datas = new Dictionary<Type, DeckDataComponent>();
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

        public void RequestDeath()
        {
            collider.enabled = false;

            foreach (var deckComponent in components)
            {
                deckComponent.OnDeath();
            }

            InternalRequestDeath();
        }

        public Transform GetCenter() => centerPosition;
        public float GetSize() => activeSize;
        public DeckAgentShape GetShape() => shape;

        protected virtual void InternalRequestDeath()
        {
            Destroy(gameObject);
        }
        public bool WillSave() => willSave;
    }
}