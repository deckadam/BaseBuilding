using System.Collections.Generic;
using Deck.Components;
using Deck.MVC;
using Deck.Utility.Logger;
using Zenject;

namespace Deck.UI.Health
{
    public class DeckHealthUI : DeckUIBase
    {
        private DeckMVCController<DeckHealthComponent, IEnumerable<DeckHealthComponent>> _uiController;
        private List<DeckHealthBar> _healthBars = new();
        private DeckHealthBar.Factory _healthBarFactory;

        [Inject]
        private void Inject(DeckHealthBar.Factory healthBarFactory)
        {
            _healthBarFactory = healthBarFactory;
        }

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

            for (var i = 0; i < _healthBars.Count; i++)
            {
                if (_healthBars[i] == null)
                {
                    continue;
                }

                _healthBars[i].Despawn();
            }

            _healthBars.Clear();

            DeckLogger.UI("Creating health bar");
            var temp = items.Getter();
            foreach (var healthComponent in temp)
            {
                var newCell = _healthBarFactory.Create();
                newCell.transform.SetParent(transform, false);
                newCell.gameObject.SetActive(true);
                newCell.Initialize(healthComponent);
                _healthBars.Add(newCell);
            }
        }

        public override bool CanDisappear()
        {
            return false;
        }
    }
}