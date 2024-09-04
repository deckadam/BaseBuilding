using Deck.EventManager;
using Deck.Utility.Logger;
using UnityEngine;

namespace Deck.Components.Building
{
    public class DeckNotificationMenu : DeckUIBase
    {
        [SerializeField] private DeckNotification notificationPrefab;
        [SerializeField] private RectTransform spawnPoint;

        public override void Initialize()
        {
            DeckEventManager.Register<DeckEventNotificationRequested>(OnNotificationRequested);
        }

        public override void DeInitialize()
        {
            DeckEventManager.Unregister<DeckEventNotificationRequested>(OnNotificationRequested);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                DeckEventNotificationRequested.Create("Test").Send();
            }
        }

        private void OnNotificationRequested(DeckEventNotificationRequested obj)
        {
            var newNotification = Instantiate(notificationPrefab, spawnPoint);
            newNotification.SetMessage(obj.message);
        }

        protected override bool CanDisappear()
        {
            return false;
        }
    }
}