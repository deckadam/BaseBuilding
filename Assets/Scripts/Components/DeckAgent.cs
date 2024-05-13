using System;
using System.Collections.Generic;
using Deck.Commands;
using Deck.InputHandling;
using Deck.Save;
using Deck.Save.Data;
using Deck.Services.MapService;
using Deck.UI.InGame;
using Deck.UI.Stats;
using Services.AgentFinder;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Deck.Agent
{
    public abstract class DeckAgent : MonoBehaviour
    {
        public event Action<float> OnAgentSizeChanged;
        public event Action<DeckAgent> OnAgentDeath;
        public event Action OnItemVisualChanged;

        [ReadOnly, SerializeField] private string prefabId;

        [SerializeField] protected Transform centerPosition;
        [SerializeField] private DeckComponent[] components;
        [SerializeField] private float activeSize = -1f;
        [SerializeField] private new Collider collider;
        [SerializeField] private bool willSave = true;
        [SerializeField] private DeckId uniqueId;

        [HideInInspector] public Transform selfTransform;

        private DeckComponentCommandProcessor _componentCommandProcessor;
        private bool _alreadyDeinitialized;
        private bool _hasBeenInitialized;
        private bool _hasSetComponents;

        private void Awake()
        {
            Initialize();
            uniqueId ??= new DeckId();
        }

        private void OnValidate()
        {
            components = GetComponents<DeckComponent>();
            collider = GetComponent<Collider>();

            if (string.IsNullOrEmpty(prefabId))
            {
                prefabId = Guid.NewGuid().ToString();
            }
        }

        private void OnDestroy()
        {
            DeInitialize();
        }

        public bool TryGetDeckComponent<T>(out T value) where T : class
        {
            for (var i = 0; i < components.Length; i++)
            {
                if (components[i] is T component)
                {
                    value = component;
                    return true;
                }
            }

            value = null;
            return false;
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

        private void SetComponents()
        {
            if (_hasSetComponents)
            {
                return;
            }

            _hasSetComponents = true;
            var components = GetComponents<DeckComponent>();
            this.components = components;
        }

        public void Initialize(string guid)
        {
            if (_hasBeenInitialized)
            {
                return;
            }

            uniqueId = new DeckId(guid);
            Initialize(false);
        }

        public void Initialize(bool createNewGuid = true)
        {
            if (_hasBeenInitialized)
            {
                return;
            }

            if (activeSize < 0f)
            {
                throw new Exception("Size value can not be lower than zero");
            }

            if (createNewGuid)
            {
                uniqueId = new DeckId();
            }

            selfTransform = transform;
            selfTransform.SetParent(DeckServiceScene.GetMap().transform, true);


            SetComponents();

            _componentCommandProcessor = GetDeckComponent<DeckComponentCommandProcessor>();

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

            AfterInitialize();

            Deck.GetService<DeckServiceFinder>().RegisterAgent(this);
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

            if (_componentCommandProcessor != null)
            {
                _componentCommandProcessor.StopExecutions();
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

                var serializedValue = DeckSaveUtility.GetSerializedData(temp);
                result.Add(new DeckComponentSaveData
                {
                    id = deckComponent.GetType().ToString(),
                    data = serializedValue,
                });
            }

            return result;
        }

        public void LoadData(DeckComponentHolderSaveData data)
        {
            Initialize();
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

            uniqueId = new DeckId(data.agentGuid);
            AfterLoad();
        }

        public void AddCommand(DeckCommand command)
        {
            _componentCommandProcessor.AddCommand(command, DeckInputHandlingSystem.IsInterruptingCommandModeActive);
        }

        public void EnqueCommand(DeckCommand command)
        {
            _componentCommandProcessor.EnqueCommand(command);
        }

        protected void SetSize(float newSize)
        {
            activeSize = newSize;
            OnAgentSizeChanged?.Invoke(activeSize);
        }

        public void RequestDeath()
        {
            if (collider != null)
            {
                collider.enabled = false;
            }

            foreach (var deckComponent in components)
            {
                deckComponent.OnDeath();
            }

            InternalRequestDeath();
            OnAgentDeath?.Invoke(this);

            Deck.GetService<DeckServiceFinder>().RemoveAgent(this);
        }

        protected virtual void InternalRequestDeath()
        {
            Destroy(gameObject);
        }

        protected void RaiseItemVisualChanged()
        {
            OnItemVisualChanged?.Invoke();
        }

        public DeckComponent[] GetDeckComponents() => components;
        public Transform GetCenter() => centerPosition;
        public float GetSize() => activeSize;
        public bool WillSave() => willSave;
        public string GetPrefabId() => prefabId;

        protected virtual void AfterInitialize()
        {
        }

        protected virtual void AfterLoad()
        {
        }

        public DeckStatGroup[] GetStats()
        {
            var compCount = components.Length;
            var stats = new DeckStatGroup[compCount];

            for (var i = 0; i < compCount; i++)
            {
                var comp = components[i];
                var statGroup = comp.GetStatGroup();
                stats[i] = statGroup;
            }

            return stats;
        }

        public DeckId GetUniqueId()
        {
            return uniqueId;
        }
    }
}