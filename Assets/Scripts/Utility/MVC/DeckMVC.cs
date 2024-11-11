using System;
using System.Collections.Generic;

namespace Deck.Utility.MVC
{
    public static class DeckMVC<T, J> where T : class where J : class
    {
        private static Dictionary<Type, DeckMVCController<T, J>> _controllers = new();

        public static DeckMVCController<T, J> GetController()
        {
            if (_controllers.TryGetValue(typeof(T), out var result))
            {
                return result;
            }

            var newController = new DeckMVCController<T, J>();
            _controllers[typeof(T)] = newController;
            return newController;
        }

        public static void ResetController()
        {
            if (_controllers == null)
            {
                _controllers = new Dictionary<Type, DeckMVCController<T, J>>();
            }
            else if (_controllers.TryGetValue(typeof(T), out var controller))
            {
                controller.Clear();
            }
        }
    }
}