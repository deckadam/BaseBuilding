using System;
using System.Collections.Generic;
using System.Linq;
using Deck.Components;
using Deck.Utility.MVC;
using Services;

namespace Deck.Services.Health
{
    public class DekcServiceHealthBar : DeckServiceBase, IDeckModel<DeckComponentHealth, IEnumerable<DeckComponentHealth>>
    {
        private List<DeckComponentHealth> _healthComponents;
        private Action<IDeckModel<DeckComponentHealth, IEnumerable<DeckComponentHealth>>> _listeners;
        private bool _hasInitialized;

        public override void Initialize()
        {
            if (_hasInitialized)
            {
                return;
            }

            _healthComponents = new List<DeckComponentHealth>();
            DeckMVC<DeckComponentHealth, IEnumerable<DeckComponentHealth>>.GetController().SetModel(this);
        }

        public void AddData(DeckComponentHealth data)
        {
            _healthComponents.Add(data);
            Raise();
        }

        public void RemoveData(DeckComponentHealth data)
        {
            _healthComponents.Remove(data);
            Raise();
        }

        public IEnumerable<DeckComponentHealth> Getter()
        {
            return _healthComponents;
        }

        public void Setter(IEnumerable<DeckComponentHealth> obj)
        {
            _healthComponents = obj.ToList();
        }

        public void Register(Action<IDeckModel<DeckComponentHealth, IEnumerable<DeckComponentHealth>>> listener)
        {
            _listeners += listener;
        }

        public void Unregister(Action<IDeckModel<DeckComponentHealth, IEnumerable<DeckComponentHealth>>> listener)
        {
            _listeners -= listener;
        }

        public void ClearListeners()
        {
            _listeners = null;
        }

        private void Raise()
        {
            _listeners?.Invoke(this);
        }
    }
}