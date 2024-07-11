using System;
using Deck.Commands;
using Deck.Data.General;
using Deck.Data.Item;
using Deck.ItemVisualProviders;
using Deck.Services;
using Deck.Services.CellSelectionService;
using Deck.UI.InGame;
using Deck.UI.Item;
using Deck.Utility.Logger;
using DG.Tweening;
using Services.AgentFinder;
using Unity.Collections;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Deck.Item
{
    public class DeckItemVisual : MonoBehaviour
    {
        [ReadOnly, SerializeField] private bool isStatic;

        [SerializeField] private Vector3 localEquipRotation;
        [SerializeField] private Vector3 localEquipPosition;
        [SerializeField] private DeckDataItem bindedItem;
        [SerializeField] private Vector3 displayOffset;
        [SerializeField] private new Rigidbody rigidbody;
        [SerializeField] private bool isOnTheGround;
        [SerializeField] private new Collider collider;
        [SerializeField] private DeckId uniqueId;
        [SerializeField] private DeckId prefabId;

        public Vector3 LocalEquipPosition => localEquipPosition;
        public Vector3 LocalEquipRotation => localEquipRotation;

        private DeckBinderGeneral.DeckGeneralData _generalData;
        private DeckFactoryProviderUI _factoryProvider;
        private DeckUIItemDisplayer _display;
        private bool _hasDropped;
        private bool _hasInitialized;
        public void SetItem(DeckDataItem item) => bindedItem = item;
        public DeckDataItem GetBindedItem() => bindedItem;
        public DeckId UniqueId => uniqueId;
        public bool IsStatic => isStatic;

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
        private void Inject(DeckFactoryProviderUI factoryProvider, DeckBinderGeneral.DeckGeneralData generalDataData)
        {
            _factoryProvider = factoryProvider;
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

        private void OnValidate()
        {
            rigidbody = GetComponent<Rigidbody>();
            isStatic = rigidbody == null;

            collider = GetComponentInChildren<Collider>();

            UniqueId.ResetId();

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

            if (_display != null)
            {
                _display.Despawn();
            }

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

            _display = _factoryProvider.GetFactory<DeckUIItemDisplayer, DeckUIItemDisplayer.Factory>().Create();
            _display.SetTarget(transform);
            _display.SetData(bindedItem, CreatePickUpCommand);
            _display.SetPositionOffset(displayOffset);

            _hasDropped = true;
            isOnTheGround = true;
            collider.enabled = true;
        }

        private void CreatePickUpCommand()
        {
            if (isStatic)
            {
                return;
            }

            var agent = DeckServiceSelection.currentPossession;
            if (agent == null)
            {
                return;
            }

            var inventory = agent.GetDeckComponent<DeckComponentInventory>();
            if (inventory == null)
            {
                return;
            }

            var movement = agent.GetDeckComponent<DeckComponentMovement>();
            if (movement == null)
            {
                return;
            }

            var command = new DeckCommandPickUpItem();
            command.Initialize(inventory, movement, this);
            agent.AddCommand(command);
        }


        public void OnEquip()
        {
            if (_display != null)
            {
                _display.Despawn();
            }

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
    }
}