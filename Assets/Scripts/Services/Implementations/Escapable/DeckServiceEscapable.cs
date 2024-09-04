using System.Collections.Generic;
using Deck.Services;
using Deck.Components.Building;
using Unity.VisualScripting;

namespace Services.Implementations.Escapable
{
    public class DeckServiceEscapable : DeckServiceBase
    {
        private Stack<IDeckEscapable> _escapables;

        public override void Initialize()
        {
            _escapables = new Stack<IDeckEscapable>();
        }

        public void RegisterEscapable(IDeckEscapable escapable)
        {
            _escapables.Push(escapable);
        }

        public void CloseEscapable()
        {
            if (_escapables.Count > 0)
            {
                var escapable = _escapables.Pop();
                escapable.OnCloseRequested();
            }
            else
            {
                Deck.Deck.GetService<DeckServiceUI>().GetUI<DeckMainMenu>().SwapAppearanceStatus();
            }
        }

        public void ClearEscapables()
        {
            while (_escapables.TryPop(out var escapable))
            {
                escapable.OnCloseRequested();
            }

            _escapables.Clear();
        }
    }
}