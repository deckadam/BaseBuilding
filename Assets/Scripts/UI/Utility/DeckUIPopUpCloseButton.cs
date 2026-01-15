using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Deck.UI.Utility
{
    public class DeckUIPopUpCloseButton : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            GetComponentInParent<DeckPopUpBase>().OnCloseRequested();
        }
    }
}