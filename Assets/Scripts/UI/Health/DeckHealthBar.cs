using Deck.Components;
using Deck.Player;
using Deck.Services.Implementations.ObjectPooling;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Deck.UI.Health
{
    public class DeckHealthBar : MonoBehaviour, IDeckPoolable
    {
        [SerializeField] private RectTransform rect;
        [SerializeField] private TextMeshProUGUI numberDisplay;
        [SerializeField] private Image fillBar;
        private Camera _mainCamera;
        private DeckAgent _agent;
        private DeckHealthComponent _healthComponent;

        [Inject]
        private void Inject(Camera camera)
        {
            _mainCamera = camera;
        }

        public void Initialize(DeckHealthComponent healthComponent)
        {
            _healthComponent = healthComponent;
            _healthComponent.Register(OnDataChanged);

            _agent = healthComponent.GetAgent();

            OnDataChanged();
        }

        private void OnDestroy()
        {
            _healthComponent?.Unregister(OnDataChanged);
        }

        private void OnDataChanged()
        {
            numberDisplay.text = _healthComponent.GetHealth().ToString();
            fillBar.fillAmount = _healthComponent.GetHealthRatio();
        }

        private void LateUpdate()
        {
            rect.position = _mainCamera.WorldToScreenPoint(_agent.transform.position + Vector3.up * 2f);
        }

        public void Initialize()
        {
        }

        public void DeInitialize()
        {
        }

        public MonoBehaviour getMonoBehaviour()
        {
            return this;
        }
    }
}