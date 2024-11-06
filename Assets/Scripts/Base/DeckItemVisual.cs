#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using Deck.Base;
using Deck.Base.Id;
using Deck.Data.General;
using Deck.Data.Item;
using Deck.ItemVisualProviders;
using Deck.Utility.Logger;
using DG.Tweening;
using Services.AgentFinder;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Deck.Components
{
    public class DeckItemVisual : DeckPoolable
    {
        [Unity.Collections.ReadOnly, SerializeField]
        private bool isStatic;

        [SerializeField] private Vector3 localEquipRotation;
        [SerializeField] private Vector3 localEquipPosition;
        [SerializeField] private DeckDataItem bindedItem;
        [SerializeField] private Vector3 displayOffset;
        [SerializeField] private new Rigidbody rigidbody;
        [SerializeField] private bool isOnTheGround;
        [SerializeField] private new Collider collider;
        [SerializeField] private float size;
        [SerializeField] private DeckId uniqueId;
        [SerializeField] private DeckId prefabId;
        [SerializeField] private bool canBePlacedOnTop;
        public DeckAgent Agent { get; set; }

        public Vector3 LocalEquipPosition => localEquipPosition;
        public Vector3 LocalEquipRotation => localEquipRotation;
        private DeckBinderGeneral.DeckGeneralData _generalData;
        public void SetItem(DeckDataItem item) => bindedItem = item;
        public DeckDataItem GetBoundItem() => bindedItem;
        public DeckId UniqueId => uniqueId;
        public Collider Collider => collider;
        public bool CanBePlacedOnTop => canBePlacedOnTop;
        private bool _hasDropped;
        private bool _hasInitialized;

        public DeckId PrefabId
        {
            get
            {
                if (prefabId.IsValid)
                {
                    return prefabId;
                }

                throw new Exception("No valid prefab id " + name + " " + prefabId.ID);
            }
        }

        [Inject]
        private void Inject(DeckBinderGeneral.DeckGeneralData generalDataData)
        {
            _generalData = generalDataData;
        }

        public void SetNewUniqueId()
        {
            uniqueId = DeckId.CreateNew();
        }

        public void SetUniqueId(DeckId id)
        {
            uniqueId = id;
        }

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (_hasInitialized)
            {
                return;
            }

            _hasInitialized = true;
            if (isOnTheGround)
            {
                if (!uniqueId.IsValid)
                {
                    uniqueId = DeckId.CreateNew();
                }

                Deck.GetService<DeckServiceFinder>().RegisterItemVisual(this);

                OnDroppped();
            }
        }

        [Button]
        private void CalculateSize()
        {
            if (collider != null)
            {
                if (collider is BoxCollider boxCollider)
                {
                    var sizes = boxCollider.bounds.size;
                    size = Mathf.Max(sizes.x, sizes.z);
                }
                else if (collider is SphereCollider sphereCollider)
                {
                    size = sphereCollider.radius;
                }
                else if (collider is CapsuleCollider capsuleCollider)
                {
                    var sizes = capsuleCollider.bounds.size;
                    size = Mathf.Max(sizes.x, sizes.y, sizes.z);
                }
                else
                {
                    DeckLogger.Error($"Invalid collider type {collider.GetType()}");
                }
            }
        }

        [Button]
        public void OnValidate()
        {
            rigidbody = GetComponent<Rigidbody>();
            isStatic = rigidbody == null;

#if UNITY_EDITOR
            if (!PrefabUtility.IsPartOfPrefabAsset(gameObject))
            {
                return;
            }
#endif

            foreach (var itemVisual in Resources.FindObjectsOfTypeAll(typeof(DeckItemVisual)))
            {
                var itemVisualComponent = itemVisual as DeckItemVisual;

                if (itemVisualComponent.name == name)
                {
                    continue;
                }

                if (itemVisualComponent.prefabId.Equals(prefabId))
                {
                    DeckLogger.Error("Multiple prefab id " + prefabId.ID + " " + name + itemVisualComponent.name);
                }
            }

            if (int.TryParse(name[^1].ToString(), out var _))
            {
                prefabId.ResetId();
                return;
            }

            if (!prefabId.IsValid)
            {
                prefabId = DeckId.CreateNew();
            }
        }

        public async void OnPickUp(Transform targetPosition)
        {
            if (!isOnTheGround)
            {
                return;
            }

            isOnTheGround = false;

            if (!isStatic)
            {
                rigidbody.isKinematic = true;
            }

            collider.enabled = false;
            await DOVirtual.Float(0f, 1f, _generalData.ItemCollectingFlyAnimation.Duration, val =>
            {
                var temp = Vector3.Lerp(transform.position, targetPosition.position, val);
                temp.y = _generalData.ItemCollectingFlyAnimation.Value.Evaluate(val);
                transform.position = temp;
            }).SetEase(_generalData.ItemCollectingFlyAnimation.Ease).AsyncWaitForCompletion();

            Deck.GetService<DeckServiceItemVisual>().ReturnItemVisual(this);
            _hasDropped = false;
        }

        public void OnDroppped()
        {
            if (isStatic)
            {
                DeckLogger.Error("Trying to drop a static item");
                return;
            }

            if (_hasDropped)
            {
                return;
            }

            _hasDropped = true;
            isOnTheGround = true;
            collider.enabled = true;
        }

        public void SetAgent(DeckAgent agent)
        {
            Agent = agent;
        }

        public void OnEquip()
        {
            if (!isStatic)
            {
                rigidbody.isKinematic = true;
            }
        }

        public void ThrowInRandomDirection(float forceMultiplier = 10f)
        {
            if (isStatic) return;

            collider.enabled = true;

            var force = Random.insideUnitSphere;
            force.y = 0.5f;
            force = force.normalized * forceMultiplier;

            rigidbody.isKinematic = false;
            rigidbody.AddForce(force, ForceMode.Impulse);
        }

        public string GetAdditionalData()
        {
            return null;
        }

        public bool IsOnTheGround => isOnTheGround;

        public float GetSize()
        {
            return size;
        }
    }
}