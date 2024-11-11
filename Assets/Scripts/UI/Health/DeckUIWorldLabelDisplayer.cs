using System.Collections.Generic;
using UnityEngine;

namespace Deck.UI.Health
{
    public class DeckUIWorldLabelDisplayer : DeckUIBase
    {
        [SerializeField] private List<DeckUIWorldDisplay> _healthBars = new();

        public override void Initialize()
        {
            if (_healthBars == null)
            {
                _healthBars = new List<DeckUIWorldDisplay>();
            }
            else
            {
                _healthBars.Clear();
            }
        }

        public void AddDisplay(DeckUIWorldDisplay healthBar)
        {
            healthBar.transform.SetParent(transform, false);
            _healthBars.Add(healthBar);
        }

        public void RemoveDisplay(DeckUIWorldDisplay healthBar)
        {
            _healthBars.Remove(healthBar);
        }

        protected override bool CanDisappear()
        {
            return false;
        }
    }
}