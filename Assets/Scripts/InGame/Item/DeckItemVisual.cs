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
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Deck.Item
{
    [RequireComponent(typeof(Collider))]
    public class DeckItemVisual : MonoBehaviour
    {
        [Unity.Collections.ReadOnly, SerializeField]
        private bool isStatic;

        [SerializeField] private Quaternion localEquipRotation;
        [SerializeField] private Vector3 localEquipPosition;
        [SerializeField] private DeckDataItem bindedItem;
        [SerializeField] private Vector3 displayOffset;
        [SerializeField] private new Rigidbody rigidbody;
        [SerializeField] private bool isOnTheGround;
        [SerializeField] private new Collider collider;
        [SerializeField] private DeckId uniqueId;
        [ReadOnly, SerializeField] private string prefabId;
        public Vector3 LocalEquipPosition => localEquipPosition;
        public Quaternion LocalEquipRotation => localEquipRotation;

        private DeckBinderGeneral.DeckGeneralData _generalData;
        private DeckFactoryProviderUI _factoryProvider;
        private DeckUIItemDisplayer _display;

        public void SetItem(DeckDataItem item) => bindedItem = item;
        public DeckDataItem GetBindedItem() => bindedItem;
        public DeckId UniqueId => uniqueId;
        private DeckId _prefabId;
        public bool IsStatic => isStatic;

        public DeckId PrefabId
        {
            get
            {
                if (_prefabId != null)
                {
                    return _prefabId;
                }

                _prefabId = new DeckId(prefabId);
                return _prefabId;
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

        public void SetId(Guid id)
        {
            uniqueId = new DeckId(id);
        }

        private void Awake()
        {
            if (isOnTheGround)
            {
                OnDroppped();
            }
        }

        private void OnValidate()
        {
            rigidbody = GetComponent<Rigidbody>();
            isStatic = rigidbody == null;

            collider = GetComponent<Collider>();

            UniqueId.ResetId();

            if (string.IsNullOrEmpty(prefabId))
            {
                prefabId = Guid.NewGuid().ToString();
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
        }

        public void OnDroppped()
        {
            if (isStatic)
            {
                DeckLogger.Error("Trying to drop a static item");
                return;
            }

            isOnTheGround = true;
            collider.enabled = true;
            _display = _factoryProvider.GetFactory<DeckUIItemDisplayer, DeckUIItemDisplayer.Factory>().Create();
            _display.SetTarget(transform);
            _display.SetData(bindedItem, CreatePickUpCommand);
            _display.SetPositionOffset(displayOffset);
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
    }
}