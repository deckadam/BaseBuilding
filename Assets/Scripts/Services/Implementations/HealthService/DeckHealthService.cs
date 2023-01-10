using System;
using System.Collections.Generic;
using System.Linq;
using Deck.Components;
using Deck.MVC;

namespace Deck.Services.Implementations.HealthService
{
    public class DeckHealthService : DeckServiceBase, IDeckModel<DeckHealthComponent, IEnumerable<DeckHealthComponent>>
    {
        private List<DeckHealthComponent> _healthComponents;
        private Action<IDeckModel<DeckHealthComponent, IEnumerable<DeckHealthComponent>>> _listeners;
        private bool _hasInitialized;

        public override void Initialize()
        {
            if (_hasInitialized)
            {
                return;
            }

            _healthComponents = new List<DeckHealthComponent>();
            DeckMVC<DeckHealthComponent, IEnumerable<DeckHealthComponent>>.GetController().SetModel(this);
        }

        public void AddData(DeckHealthComponent data)
        {
            _healthComponents.Add(data);
            Raise();
        }

        public void RemoveData(DeckHealthComponent data)
        {
            _healthComponents.Remove(data);
            Raise();
        }

        public IEnumerable<DeckHealthComponent> Getter()
        {
            return _healthComponents;
        }

        public void Setter(IEnumerable<DeckHealthComponent> obj)
        {
            _healthComponents = obj.ToList();
        }

        public void Register(Action<IDeckModel<DeckHealthComponent, IEnumerable<DeckHealthComponent>>> listener)
        {
            _listeners += listener;
        }

        public void Unregister(Action<IDeckModel<DeckHealthComponent, IEnumerable<DeckHealthComponent>>> listener)
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