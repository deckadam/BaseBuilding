using System.Collections.Generic;
using Deck.Components;
using Deck.MVC;
using Deck.Services;
using Deck.Services.Implementations.ObjectPooling;
using Deck.Utility.Logger;

namespace Deck.UI.Health
{
    public class DeckHealthUI : DeckUIBase
    {
        private DeckPoolingService _poolingService;
        private DeckMVCController<DeckHealthComponent, IEnumerable<DeckHealthComponent>> _uiController;
        private List<DeckHealthBar> _healthComponents = new();

        public override void Initialize()
        {
            _poolingService = DeckServiceLocator.GetService<DeckPoolingService>();
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
                _healthComponents[i].gameObject.SetActive(false);
                _poolingService.ReturnToPool(_healthComponents[i]);
            }

            _healthComponents.Clear();

            DeckLogger.UI("Creating health bar");
            var temp = items.Getter();
            foreach (var healthComponent in temp)
            {
                var newCell = _poolingService.GetFromPool<DeckHealthBar>();
                newCell.transform.SetParent(transform, false);
                newCell.gameObject.SetActive(true);
                newCell.Initialize(healthComponent);
                _healthComponents.Add(newCell);
            }
        }

        public override bool CanDisappear()
        {
            return false;
        }
    }
}