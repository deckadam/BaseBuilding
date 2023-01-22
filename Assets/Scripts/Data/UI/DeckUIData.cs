using Deck.UI.Inventory;
using Deck.UI.SaveListingMenu;
using UnityEngine;
using Zenject;

namespace Deck.Data.UI
{
    [CreateAssetMenu(fileName = "Deck UI Data", menuName = "Deck/Installer/UI", order = 0)]
    public class DeckUIData : ScriptableObjectInstaller
    {
        [SerializeField] private DeckInventoryDisplayerCell cellPrefab;
        [SerializeField] private DeckSaveDisplayer saveDisplayerPrefab;
        [SerializeField] private float canvasAppearDuration;
        [SerializeField] private float canvasDisapearDuration;


        public override void InstallBindings()
        {
            Container.BindInstance(this);
        }

        public DeckInventoryDisplayerCell GetCellPrefab() => cellPrefab;
        public DeckSaveDisplayer GetSaveDisplayerPrefab() => saveDisplayerPrefab;
        public float GetCanvasAppearDuration() => canvasAppearDuration;
        public float GetCanvasDisappearDuration() => canvasDisapearDuration;
    }
}