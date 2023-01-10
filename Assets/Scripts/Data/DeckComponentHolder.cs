using System.Linq;
using Deck.Components;

namespace Deck.Test.Data
{
    public class DeckComponentHolder
    {
        private IDeckComponent[] _components;

        protected void SetComponents(params IDeckComponent[] components)
        {
            _components = components;
        }

        public IDeckComponent GetComponent<T>() where T : IDeckComponent
        {
            return (T) _components.Where(item => item.GetType() == typeof(T));
        }
    }
}