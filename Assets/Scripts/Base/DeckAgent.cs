using System;
using System.Collections.Generic;
using Components;
using Cysharp.Threading.Tasks;
using Data.Agent;
using General;
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
    public class DeckAgent : DeckPoolable, IDeckTranslatable
    {
        public event Action OnItemVisualChanged;

        [SerializeField, ReadOnly] protected DeckItemVisual[] itemVisualInstances;
        [SerializeField, ReadOnly] protected DeckItemVisual itemVisualInstance;
        [SerializeField, ReadOnly] protected bool isMultiInstance;

        [SerializeField] private DeckDataAgent[] allData;
        [SerializeField] protected DeckActionTag[] tags;
        [SerializeField] private DeckComponent[] components;
        [SerializeField] private bool willSave = true;
        [SerializeField] private DeckId uniqueId;
        [SerializeField] private DeckId prefabId;

        public DeckId UniqueId => uniqueId;
        public DeckComponent[] Components => components;
        public bool WillSave => willSave;

        protected Transform SelfTransform;

        private Dictionary<Type, DeckDataAgent> _allData;
        private bool _alreadyDeInitialized;
        private bool _hasBeenInitialized;

        private bool _isSpawned;

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
        [Button]
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

                if (agentComponent == this)
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
            uniqueId = new DeckId(id);
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

            _hasBeenInitialized = true;

            _allData = new Dictionary<Type, DeckDataAgent>();
            foreach (var deckDataAgent in allData)
            {
                Debug.LogError(deckDataAgent.GetType());
                _allData.Add(deckDataAgent.GetType(), deckDataAgent);
            }
            
            OnSpawned();

            AfterInitialize();
        }

        protected virtual void AfterInitialize()
        {
        }

        public override void OnSpawned()
        {
            if (_isSpawned)
            {
                return;
            }

            SelfTransform.SetParent(DeckServiceSession.GetSession().GetSelfTransform(), true);
            _isSpawned = true;
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
            AfterSpawned();
        }

        private void DeInitialize()
        {
            if (_alreadyDeInitialized)
            {
                return;
            }

            _alreadyDeInitialized = true;
            _hasBeenInitialized = false;

            OnDeSpawned();
        }

        public sealed override void OnDeSpawned()
        {
            if (!_isSpawned)
            {
                return;
            }

            _isSpawned = false;
            foreach (var deckComponent in components)
            {
                deckComponent.DeInitialize();
            }

            if (TryGetDeckComponent<DeckComponentCommandProcessor>(out var commandProcessor))
            {
                commandProcessor.StopExecutions();
            }

            InternalOnDeSpawned();
        }

        protected virtual void InternalOnDeSpawned()
        {
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

        protected virtual void AfterSpawned()
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

        public T GetAgentData<T>() where T : DeckDataAgent
        {
            Debug.LogError(typeof(T));
            return _allData[typeof(T)] as T;
        }

        public virtual object GetAdditionalData()
        {
            return string.Empty;
        }

        public Vector3 GetPosition()
        {
            return SelfTransform.position;
        }

        public virtual void SetPosition(Vector3 position)
        {
            SelfTransform.position = position;
        }

        public virtual void SetPosition(IDeckTranslatable positioner)
        {
            SelfTransform.position = positioner.GetPosition();
        }

        public Quaternion GetRotation()
        {
            return SelfTransform.rotation;
        }

        public Vector3 GetRotationEuler()
        {
            return SelfTransform.eulerAngles;
        }

        public void SetRotation(Vector3 rotation)
        {
            SelfTransform.eulerAngles = rotation;
        }

        public void SetRotation(Quaternion rotation)
        {
            SelfTransform.rotation = rotation;
        }

        public void SetRotation(IDeckTranslatable rotater)
        {
            SelfTransform.rotation = rotater.GetRotation();
        }

        public Vector3 GetScale()
        {
            return SelfTransform.localScale;
        }

        public void SetScale(Vector3 scale)
        {
            SelfTransform.localScale = scale;
        }

        public void SetScale(IDeckTranslatable scaler)
        {
            SelfTransform.localScale = scaler.GetScale();
        }
    }
}