using Deck.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Deck.UI.Health
{
    public class DeckHealthBar : MonoBehaviour, IPoolable<IMemoryPool>
    {
        [SerializeField] private RectTransform rect;
        [SerializeField] private TextMeshProUGUI numberDisplay;
        [SerializeField] private Image fillBar;
        private Camera _mainCamera;
        private Transform _target;
        private DeckHealthComponent _healthComponent;
        private IMemoryPool _memory;

        [Inject]
        private void Inject(Camera camera)
        {
            _mainCamera = camera;
        }

        public void Initialize(DeckHealthComponent healthComponent)
        {
            _healthComponent = healthComponent;
            _healthComponent.Register(OnDataChanged);

            _target = healthComponent.GetComponentOwner().transform;

            OnDataChanged();
        }

        private void OnDataChanged()
        {
            numberDisplay.text = _healthComponent.GetHealth().ToString();
            fillBar.fillAmount = _healthComponent.GetHealthRatio();
        }

        private void LateUpdate()
        {
            rect.position = _mainCamera.WorldToScreenPoint(_target.transform.position + Vector3.up * 2f);
        }

        public void Despawn()
        {
            _memory.Despawn(this);
        }

        public void OnDespawned()
        {
            _memory = null;
            _healthComponent?.Unregister(OnDataChanged);
        }

        public void OnSpawned(IMemoryPool p1)
        {
            _memory = p1;
        }

        public class Factory : PlaceholderFactory<DeckHealthBar>
        {
        }
    }
}