using System.Collections.Generic;
using Deck.Components;
using Deck.MVC;
using Deck.Utility.Logger;
using UnityEngine;

namespace Deck.UI.Health
{
    public class DeckHealthUI : DeckUIBase
    {
        private DeckMVCController<DeckHealthComponent, IEnumerable<DeckHealthComponent>> _uiController;
        private List<GameObject> _healthComponents = new();

        public override void Initialize()
        {
            _uiController = DeckMVC<DeckHealthComponent, IEnumerable<DeckHealthComponent>>.GetController();
            _uiController.AddModelListener(CreateNewCells);
        }

        public override void DeInitialize()
        {
            _uiController.RemoveModelListener(CreateNewCells);
        }

        private void CreateNewCells(IDeckModel<DeckHealthComponent, IEnumerable<DeckHealthComponent>> items)
        {
            if (!_isAppeared && !isAppearing)
            {
                return;
            }

            for (var i = 0; i < _healthComponents.Count; i++)
            {
                Destroy(_healthComponents[i]);
            }

            _healthComponents.Clear();

            DeckLogger.UI("Creating health bar");
            var temp = items.Getter();
            foreach (var healthComponent in temp)
            {
                var newCell = container.InstantiatePrefab(uiData.healthBar, transform).GetComponent<DeckHealthBar>();
                newCell.Initialize(healthComponent);
                _healthComponents.Add(newCell.gameObject);
            }
        }

        public override bool CanDisappear()
        {
            return false;
        }
    }
}