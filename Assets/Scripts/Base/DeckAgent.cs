using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Deck.Base;
using Deck.Components.Building.InGame;
using Deck.Components.Building.Stats;
using Deck.InputHandling;
using Deck.Save;
using Deck.Services.MapService;
using Deck.Utility.Logger;
using Services.AgentFinder;
using UnityEditor;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEngine;
#endif

namespace Deck.Components
{
    public class DeckAgent : DeckPoolable
    {
        public event Action<float> OnAgentSizeChanged;
        public event Action<DeckAgent> OnAgentDestroyed;
        public event Action OnItemVisualChanged;

        [SerializeField] protected Transform centerPosition;
        [SerializeField] private DeckComponent[] components;
        [SerializeField] private float activeSize = -1f;
        [SerializeField] private new Collider collider;
        [SerializeField] private bool willSave = true;
        [SerializeField] private DeckId uniqueId;
        [SerializeField] private DeckId prefabId;

        [FormerlySerializedAs("itemVisualInstance")] [FormerlySerializedAs("ItemVisualInstance")] [SerializeField]
        protected DeckItemVisual itemVisualPrefab;

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

        public void OnValidate()
        {
            components = GetComponents<DeckComponent>();
            collider = GetComponent<Collider>();

#if UNITY_EDITOR
            if (!PrefabUtility.IsPartOfPrefabAsset(gameObject))
            {
                return;
            }
#endif

            foreach (var agent in Resources.FindObjectsOfTypeAll(typeof(DeckAgent)))
            {
                var agentComponent = agent as DeckAgent;

                if (agentComponent == null)
                {
                    continue;
                }

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

            if (int.TryParse(name[..1], out _))
            {
                Debug.LogError("Resetting id " + name + " " + prefabId.ID);
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

        public void SetNewUniqueId()
        {
            uniqueId = DeckId.CreateNew();
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
            
            
            if (activeSize < 0f)
            {
                throw new Exception("Size value can not be lower than zero  "  + name);
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

        public void EnqueueCommand(DeckCommand command)
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

        public void RequestDestroy()
        {
            if (collider)
            {
                collider.enabled = false;
            }

            foreach (var deckComponent in components)
            {
                deckComponent.OnDeath();
            }

            InternalRequestDestroy();
            OnAgentDestroyed?.Invoke(this);

            Deck.GetService<DeckServiceFinder>().RemoveAgent(this);
        }

        protected virtual void InternalRequestDestroy()
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

        public void SetItemVisual(DeckItemVisual itemVisualPrefab)
        {
            this.itemVisualPrefab = itemVisualPrefab;
        }
    }
}