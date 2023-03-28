using Deck.Components;
using Deck.Animators;
using Deck.Data.Item;
using Deck.Events;
using Deck.Events.CellSelectionService;
using Deck.UI.Item;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Deck.Item
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class DeckItemVisual : MonoBehaviour
    {
        [SerializeField] private DeckAnimationParametersAnimationCurve animationParameters;
        [SerializeField] private DeckDataItem bindedItem;
        [SerializeField] private new Rigidbody rigidbody;
        [SerializeField] private new Collider collider;
        [SerializeField] private Vector3 displayOffset;
        [SerializeField] private Vector3 localEquipPosition;
        [SerializeField] private Quaternion localEquipRotation;

        private DeckFactoryProviderUI _factoryProvider;
        private DeckUIItemDisplayer _display;

        public Vector3 LocalEquipPosition => localEquipPosition;
        public Quaternion LocalEquipRotation => localEquipRotation;

        [Inject]
        private void Inject(DeckFactoryProviderUI factoryProvider)
        {
            _factoryProvider = factoryProvider;
        }

        public void OnDroppped()
        {
            _display = _factoryProvider.GetFactory<DeckUIItemDisplayer, DeckUIItemDisplayer.Factory>().Create();
            _display.SetTarget(transform);
            _display.SetData(bindedItem);
            _display.SetPositionOffset(displayOffset);
        }


        private void OnValidate()
        {
            rigidbody = GetComponent<Rigidbody>();
            collider = GetComponent<Collider>();
        }

        public void ThrowInRandomDirection(float forceMultiplier = 2f)
        {
            collider.enabled = true;
            rigidbody.isKinematic = false;
            var force = Random.insideUnitSphere;
            force.y = 0.5f;
            force = force.normalized * forceMultiplier;

            rigidbody.AddForce(force, ForceMode.Impulse);
        }

        public void PickUp()
        {
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

            agent.AddCommand(new DeckCommandPickUpItem(inventory, movement, this));
        }

        public async void OnPickUp(Transform targetPosition)
        {
            rigidbody.isKinematic = true;
            await DOVirtual.Float(0f, 1f, animationParameters.Duration, val =>
            {
                var temp = Vector3.Lerp(transform.position, targetPosition.position, val);
                temp.y = animationParameters.Value.Evaluate(val);
                transform.position = temp;
            }).SetEase(animationParameters.Ease).AsyncWaitForCompletion();
            _display.Despawn();
            Destroy(gameObject);
        }

        public void OnEquip()
        {
            _display.Despawn();
            rigidbody.isKinematic = true;
        }

        public DeckDataItem GetItem() => Instantiate(bindedItem);
    }
}