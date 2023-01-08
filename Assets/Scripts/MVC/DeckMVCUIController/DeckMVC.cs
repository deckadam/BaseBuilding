using System;
using System.Collections.Generic;
using Deck.Test.MVC.DeckMVCController;

namespace MVC.DeckMVCUIController
{
    public static class DeckMVC<T> where T : class
    {
        private static Dictionary<Type, DeckMVCController<T>> _controllers = new();

        public static DeckMVCController<T> GetController()
        {
            if (_controllers.TryGetValue(typeof(T), out var result))
            {
                return result;
            }

            var newController = new DeckMVCController<T>();
            _controllers[typeof(T)] = newController;
            return newController;
        }

        public static void ResetController()
        {
            if (_controllers == null)
            {
                _controllers = new Dictionary<Type, DeckMVCController<T>>();
            }
            else if (_controllers.TryGetValue(typeof(T), out var controller))
            {
                controller.Clear();
            }
        }
    }
}