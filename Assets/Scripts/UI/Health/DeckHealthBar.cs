using Deck.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Deck.Utility.Constants.Health
{
    public class DeckHealthBar : MonoBehaviour, IPoolable<IMemoryPool>
    {
        [SerializeField] private RectTransform rect;
        [SerializeField] private TextMeshProUGUI numberDisplay;
        [SerializeField] private Image fillBar;
        private Camera _mainCamera;
        private Transform _target;
        private DeckComponentHealth _componentHealth;
        private IMemoryPool _memory;

        [Inject]
        private void Inject(Camera camera)
        {
            _mainCamera = camera;
        }

        public void Initialize(DeckComponentHealth componentHealth)
        {
            _componentHealth = componentHealth;
            _componentHealth.Register(OnDataChanged);

            _target = componentHealth.GetComponentHolder().transform;

            OnDataChanged();
        }

        private void OnDataChanged()
        {
            numberDisplay.text = _componentHealth.GetHealth().ToString();
            fillBar.fillAmount = _componentHealth.GetHealthRatio();
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
            _componentHealth?.Unregister(OnDataChanged);
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