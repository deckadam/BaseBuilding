using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Deck.Commands;
using Deck.InputHandling;
using Deck.Item;
using Deck.Save;
using Deck.Save.Data;
using Deck.Services.MapService;
using Deck.UI.InGame;
using Deck.UI.Stats;
using Deck.Utility.Logger;
using Services.AgentFinder;
using UnityEditor;
#if UNITY_EDITOR
using UnityEngine;
#endif

namespace Deck.Agent
{
    public abstract class DeckAgent : MonoBehaviour
    {
        public event Action<float> OnAgentSizeChanged;
        public event Action<DeckAgent> OnAgentDeath;
        public event Action OnItemVisualChanged;

        [SerializeField] protected Transform centerPosition;
        [SerializeField] private DeckComponent[] components;
        [SerializeField] private float activeSize = -1f;
        [SerializeField] private new Collider collider;
        [SerializeField] private bool willSave = true;
        [SerializeField] private DeckId uniqueId;
        [SerializeField] private DeckId prefabId;

        [HideInInspector] public Transform selfTransform;

        private bool _alreadyDeinitialized;
        private bool _hasBeenInitialized;
        private bool _hasSetComponents;

        public DeckId PrefabId
        {
            get
            {
                if (prefabId.IsValid)
                {
                    return prefabId;
                }

                throw new Exception("No valid prefab id");
            }
        }
        
        private void OnValidate()
        {
            components = GetComponents<DeckComponent>();
            collider = GetComponent<Collider>();

#if UNITY_EDITOR
            if (PrefabUtility.GetPrefabParent(gameObject) == null && !PrefabUtility.IsPartOfPrefabAsset(gameObject))
            {
                return;
            }
#endif

            foreach (var agent in Resources.FindObjectsOfTypeAll(typeof(DeckAgent)))
            {
                var agentComponent = agent as DeckAgent;

                if (agentComponent.GetHashCode() == GetHashCode())
                {
                    continue;
                }

                if (agentComponent.name == name)
                {
                    continue;
                }

                if (agentComponent.prefabId.Equals(prefabId))
                {
                    DeckLogger.Error("Duplicate prefab id " + prefabId.ID + " " + name + " " + agentComponent.name, gameObject);
                }
            }

            if (int.TryParse(name[..1], out var _))
            {
                Debug.LogError("Reseting id " + name + " " + prefabId.ID);
                prefabId.ResetId();
                return;
            }

            if (!prefabId.IsValid)
            {
                prefabId = DeckId.CreateNew();
            }
        }

        private void OnDestroy()
        {
            DeInitialize();
        }

        private async void Start()
        {
            await UniTask.NextFrame();
            Initialize();
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

        public void Initialize(int guid)
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
                uniqueId = DeckId.CreateNew();
            }

            selfTransform = transform;
            selfTransform.SetParent(DeckServiceScene.GetMap().transform, true);

            SetComponents();

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

            if (TryGetDeckComponent<DeckComponentCommandProcessor>(out var commandProcessor))
            {
                commandProcessor.StartProcessCommands();
            }

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
                throw new Exception($"Components hasn't been set {name}");
            }

            _alreadyDeinitialized = true;

            foreach (var deckComponent in components)
            {
                deckComponent.DeInitialize();
            }

            if (TryGetDeckComponent<DeckComponentCommandProcessor>(out var commandProcessor))
            {
                commandProcessor.StopExecutions();
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
            transform.position = data.position;
            transform.eulerAngles = data.rotation;
            transform.localScale = data.scale;
            
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

            uniqueId = new DeckId(data.uniqueId);
            AfterLoad();
        }

        public void AddCommand(DeckCommand command)
        {
            if (TryGetDeckComponent<DeckComponentCommandProcessor>(out var commandProcessor))
            {
                commandProcessor.AddCommand(command, DeckInputHandlingSystem.IsInterruptingCommandModeActive);
            }
        }

        public void EnqueCommand(DeckCommand command)
        {
            if (TryGetDeckComponent<DeckComponentCommandProcessor>(out var commandProcessor))
            {
                commandProcessor.EnqueCommand(command);
            }
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

        public DeckId GetUniqueId() => uniqueId;
        public DeckComponent[] GetDeckComponents() => components;
        public Transform GetCenter() => centerPosition;
        public float GetSize() => activeSize;
        public bool WillSave() => willSave;
        public string GetPrefabId() => prefabId.ID.ToString();

        protected virtual void AfterInitialize()
        {
        }

        protected virtual void AfterLoad()
        {
        }
    }
}