using System;
using System.Collections.Generic;
using Components;
using Cysharp.Threading.Tasks;
using Instancing;
using Services;
using Services.Finder;
using Services.ItemVisual;
using Services.Map;
using Sirenix.OdinInspector;
using Systems.SystemSave;
using Systems.SystemSave.Data;
using UI.Stats;
using UnityEditor;
using UnityEngine;
using Utility;
using Zenject;

namespace Base
{
    public class DeckAgent : DeckPoolable
    {
        public event Action OnItemVisualChanged;

        [SerializeField, ReadOnly] protected DeckItemVisual[] itemVisualInstances;
        [SerializeField, ReadOnly] protected DeckItemVisual itemVisualInstance;
        [SerializeField, ReadOnly] protected bool isMultiInstance;

        [SerializeField] protected DeckActionTag[] tags;
        [SerializeField] private DeckComponent[] components;
        [SerializeField] private bool willSave = true;
        [SerializeField] private DeckId uniqueId;
        [SerializeField] private DeckId prefabId;

        public DeckId UniqueId => uniqueId;
        public DeckComponent[] Components => components;
        public bool WillSave => willSave;

        protected Transform SelfTransform;

        private bool _alreadyDeInitialized;
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

#if UNITY_EDITOR
        public void OnValidate()
        {
            components = GetComponents<DeckComponent>();

            if (!PrefabUtility.IsPartOfPrefabAsset(gameObject))
            {
                return;
            }

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

                if (agentComponent.GetInstanceID() == GetInstanceID())
                {
                    continue;
                }

                if (agentComponent.prefabId.Equals(prefabId))
                {
                    DeckLogger.Error("Duplicate prefab id " + prefabId.Id + " " + name + " " + agentComponent.name, gameObject);
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
#endif

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

        public void SetNewUniqueId(int id)
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
            SelfTransform.SetParent(DeckServiceSession.GetSession().transform, true);

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
                commandProcessor.StartProcessingCommands();
            }

            DeckServiceProvider.GetService<DeckServiceFinder>().RegisterAgent(this);

            AfterInitializationCompleted();
        }

        private void DeInitialize()
        {
            if (_alreadyDeInitialized)
            {
                return;
            }

            _alreadyDeInitialized = true;
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

        public List<DeckComponentSaveData> GetComponentData()
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

        public void RequestDestroy()
        {
            foreach (var deckComponent in components)
            {
                deckComponent.OnDestroy();
            }


            DeckServiceProvider.GetService<DeckServiceFinder>().RemoveAgent(this);

            if (isMultiInstance && itemVisualInstances != null)
            {
                DeckServiceProvider.GetService<DeckServiceItemVisual>().ReturnItemVisuals(itemVisualInstances);
            }
            else if (itemVisualInstance != null)
            {
                DeckServiceProvider.GetService<DeckServiceItemVisual>().ReturnItemVisual(itemVisualInstance);
            }

            instanceProvider.ReturnAgent(this);


            OnAgentDestroyed();

            InternalRequestDestroy();

            DeInitialize();
        }

        protected virtual void InternalRequestDestroy()
        {
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

        public void SetItemVisuals(DeckItemVisual[] newInstances)
        {
            itemVisualInstances = newInstances;
            foreach (var deckItemVisual in newInstances)
            {
                deckItemVisual.transform.parent = SelfTransform;
                deckItemVisual.transform.localPosition = Vector3.zero;
            }

            isMultiInstance = true;
        }

        public DeckItemVisual GetItemVisual()
        {
            return itemVisualInstance;
        }


        public DeckItemVisual[] GetItemVisuals()
        {
            return itemVisualInstances;
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