using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Deck.Commands;
using Deck.Components;
using Deck.Instancing;
using Deck.Save;
using Deck.Services.Finder;
using Deck.Services.Map;
using Deck.UI.Stats;
using Deck.Utility;
using Sirenix.OdinInspector;
using UnityEditor;
using Zenject;
#if UNITY_EDITOR
using UnityEngine;
#endif

namespace Deck.Base
{
    public class DeckAgent : DeckPoolable
    {
        public event Action OnItemVisualChanged;

        [SerializeField] protected DeckActionTag[] tags;
        [SerializeField] private DeckComponent[] components;
        [SerializeField] private bool willSave = true;
        [SerializeField] private DeckId uniqueId;
        [SerializeField] private DeckId prefabId;

        [SerializeField, ReadOnly] protected DeckItemVisual itemVisualInstance;

        public DeckId UniqueId => uniqueId;
        public DeckComponent[] Components => components;
        public bool WillSave => willSave;

        protected Transform SelfTransform;

        private bool _alreadyDeinitialized;
        private bool _hasBeenInitialized;

        protected DeckInstanceProvider instanceProvider;

        [Inject]
        private void Inject(DeckInstanceProvider instanceProvider)
        {
            this.instanceProvider = instanceProvider;
        }

        public DeckId PrefabId
        {
            get
            {
                if (prefabId.IsValid)
                {
                    return prefabId;
                }

                throw new Exception("No valid prefab id " + name);
            }
        }


        public void OnValidate()
        {
            components = GetComponents<DeckComponent>();

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
                prefabId.ResetId();
                return;
            }

            if (!prefabId.IsValid)
            {
                prefabId = DeckId.CreateNew();
            }
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

            SelfTransform = transform;
            SelfTransform.SetParent(DeckServiceScene.GetMap().transform, true);

            _hasBeenInitialized = true;

            foreach (var deckComponent in components)
            {
                deckComponent.PreInitialize(this);
            }

            foreach (var deckComponent in components)
            {
                deckComponent.PostInitialize();
            }

            if (TryGetDeckComponent<DeckComponentCommandProcessor>(out var commandProcessor))
            {
                commandProcessor.StartProcessCommands();
            }

            Deck.GetService<DeckServiceFinder>().RegisterAgent(this);

            AfterInitializationCompleted();
        }

        private void DeInitialize()
        {
            if (_alreadyDeinitialized)
            {
                return;
            }

            _alreadyDeinitialized = true;
            _hasBeenInitialized = false;

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

        public async void LoadData(DeckComponentHolderSaveData data)
        {
            SelfTransform.position = data.position;
            SelfTransform.eulerAngles = data.rotation;
            SelfTransform.localScale = data.scale;

            await UniTask.NextFrame();

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

            LoadAdditionalData(data.additionalData);
            uniqueId = new DeckId(data.uniqueId);
            AfterLoad();
        }

        public virtual void AfterLoadingFinished()
        {
        }

        public void AddCommand(DeckCommand command, bool isInterruptingCommand)
        {
            if (TryGetDeckComponent<DeckComponentCommandProcessor>(out var commandProcessor))
            {
                commandProcessor.AddCommand(command, isInterruptingCommand);
            }
        }

        public void EnqueueCommand(DeckCommand command)
        {
            if (TryGetDeckComponent<DeckComponentCommandProcessor>(out var commandProcessor))
            {
                commandProcessor.EnqueCommand(command);
            }
        }

        public void RequestDestroy()
        {
            foreach (var deckComponent in components)
            {
                deckComponent.OnDestroy();
            }


            Deck.GetService<DeckServiceFinder>().RemoveAgent(this);

            OnAgentDestroyed();

            InternalRequestDestroy();

            DeInitialize();
        }

        protected virtual void InternalRequestDestroy()
        {
            throw new Exception("Not implemented");
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

        protected virtual void AfterLoad()
        {
        }

        protected virtual void OnAgentDestroyed()
        {
        }

        protected virtual void LoadAdditionalData(string data)
        {
        }

        protected virtual void AfterInitializationCompleted()
        {
        }

        public void SetItemVisual(DeckItemVisual newInstance)
        {
            itemVisualInstance = newInstance;
            itemVisualInstance.transform.parent = SelfTransform;
            itemVisualInstance.transform.localPosition = Vector3.zero;
        }

        public DeckItemVisual GetItemVisual()
        {
            return itemVisualInstance;
        }

        public void SetTags(DeckActionTag[] tags)
        {
            this.tags = tags;
        }

        public DeckActionTag[] GetTags()
        {
            return tags;
        }

        public virtual string GetAdditionalData()
        {
            return string.Empty;
        }
    }
}