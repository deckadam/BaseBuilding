using System.Collections.Generic;
using Deck.Components;

namespace Deck.UI.Stats
{
    public class DeckUIStatContainer : DeckUIElement
    {
        private List<DeckUIStatElement> _activeElements;

        public void SetStats(DeckStat[] stats)
        {
            ClearStatElements();

            foreach (var stat in stats)
            {
                var statElement = InstanceProvider.RentUIElement<DeckUIStatElement>();
                statElement.SetStat(stat);
                statElement.transform.SetParent(transform);
                _activeElements.Add(statElement);
            }
        }

        private void ClearStatElements()
        {
            if (_activeElements.Count <= 0) return;

            InstanceProvider.ReturnUIElement(_activeElements);
            _activeElements.Clear();
        }

        protected override void InternalOnDespawned()
        {
            ClearStatElements();
        }
    }
}