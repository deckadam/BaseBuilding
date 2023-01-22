using Deck.SaveService;
using Deck.UI.SaveListingMenu.Events;
using Deck.Utility.Logger;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Deck.UI.SaveListingMenu
{
    public class DeckSaveDisplayer : MonoBehaviour, IPoolable<IMemoryPool>, IPointerClickHandler
    {
        public static DeckSaveDisplayer currentlySelectedDsiplayer => _currentlySelectedDisplayer;
        [ClearOnReload] private static DeckSaveDisplayer _currentlySelectedDisplayer;

        [SerializeField] private TextMeshProUGUI displayText;

        private DeckSaveManager.SaveFile _saveFile;
        private IMemoryPool _pool;

        public void SetData(DeckSaveManager.SaveFile saveFile)
        {
            _saveFile = saveFile;
            displayText.text = saveFile.name;
        }

        public void ReturnToPool()
        {
            _pool.Despawn(this);
        }

        public void OnDespawned()
        {
        }

        public void OnSpawned(IMemoryPool p1)
        {
            _pool = p1;
        }

        public class Factory : PlaceholderFactory<DeckSaveDisplayer>
        {
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _currentlySelectedDisplayer = this;

            if (eventData.clickCount == 2)
            {
                OnLoadRequestedEvent.Create(_saveFile).Send();
            }
        }

        public DeckSaveManager.SaveFile GetSaveFile() => _saveFile;
    }
}