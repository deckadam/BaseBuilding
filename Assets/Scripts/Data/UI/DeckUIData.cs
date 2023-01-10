using Deck.UI.Health;
using Deck.UI.Inventory;
using UnityEngine;
using Zenject;

namespace Deck.Data.UI
{
    [CreateAssetMenu(fileName = "Deck UI Data", menuName = "Deck/Installer/UI", order = 0)]
    public class DeckUIData : ScriptableObjectInstaller
    {
        public DeckInventoryDisplayerCell cellPrefab;
        public DeckHealthBar healthBar;
        public float canvasAppearDuration;
        public float canvasDisapearDuration;


        public override void InstallBindings()
        {
            Container.BindInstance(this);
        }
    }
}