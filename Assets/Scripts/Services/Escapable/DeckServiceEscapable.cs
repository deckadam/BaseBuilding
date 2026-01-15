using System.Collections.Generic;
using Deck.Services.UI;
using UI.MainMenu;

namespace Services.Escapable
{
    public class DeckServiceEscapable : DeckServiceBase
    {
        private List<IDeckEscapable> _escapables;

        public override void Initialize()
        {
            _escapables = new List<IDeckEscapable>();
        }

        public override void DeInitialize()
        {
            ClearEscapables();
        }

        public void RegisterEscapable(IDeckEscapable escapable)
        {
            _escapables.Add(escapable);
        }

        public void CloseEscapable()
        {
            if (_escapables.Count > 0)
            {
                var hasEscaped = false;
                do
                {
                    if (_escapables.Count == 0)
                    {
                        DeckServiceProvider.GetService<DeckServiceUI>().GetUI<DeckUIMainMenu>().SwapAppearanceStatus();
                        return;
                    }

                    var escapable = _escapables[^1];
                    if (!escapable.HasEscaped)
                    {
                        hasEscaped = true;
                        escapable.OnEscapeRequested();
                    }

                    _escapables.Remove(escapable);
                } while (!hasEscaped);
            }
            else
            {
                DeckServiceProvider.GetService<DeckServiceUI>().GetUI<DeckUIMainMenu>().SwapAppearanceStatus();
            }
        }

        public void RemoveEscapable(IDeckEscapable escapable)
        {
            if (_escapables.Contains(escapable))
            {
                _escapables.Remove(escapable);
                escapable.OnEscapeRequested();
            }
            else if (_escapables.Count == 0)
            {
                DeckServiceProvider.GetService<DeckServiceUI>().GetUI<DeckUIMainMenu>().SwapAppearanceStatus();
            }
        }

        public void ClearEscapables()
        {
            foreach (var deckEscapable in _escapables)
            {
                deckEscapable.OnEscapeRequested();
            }

            _escapables.Clear();
        }
    }
}