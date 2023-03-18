using Deck.EventManager;
using Deck.Utility.Logger;
using UnityEngine;

namespace Deck.Utility.Constants
{
    public class DeckNotificationMenu : DeckUIBase
    {
        [SerializeField] private DeckNotification notificationPrefab;
        [SerializeField] private RectTransform spawnPoint;

        public override void Initialize()
        {
            DeckEventManager.Register<DeckNotificationRequestedEvent>(OnNotificationRequested);
        }

        public override void DeInitialize()
        {
            DeckEventManager.Unregister<DeckNotificationRequestedEvent>(OnNotificationRequested);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                DeckNotificationRequestedEvent.Create("Test").Send();
            }
        }

        private void OnNotificationRequested(DeckNotificationRequestedEvent obj)
        {
            var newNotification = Instantiate(notificationPrefab, spawnPoint);
            newNotification.SetMessage(obj.message);
        }

        public override bool CanDisappear()
        {
            return false;
        }
    }
}