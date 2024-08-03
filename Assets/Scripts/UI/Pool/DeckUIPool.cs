using System;
using System.Collections.Generic;
using Deck.Save.Data;
using Deck.UI.InGame;
using Deck.Utility.Poolable;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Deck.UI.Pool
{
    [CreateAssetMenu(menuName = "Deck/UI/Pool", fileName = "Deck UI Pool")]
    public class DeckUIPool : ScriptableObject
    {
        [SerializeField] private List<DeckUIElement> uiElements;

        private Dictionary<DeckId, Stack<DeckUIElement>> _inPoolElements;
        private Dictionary<Type, DeckId> _inPoolElementsTypeBased;
        private DeckInstanceCreator _instanceCreator;

        [Inject]
        private void Inject(DeckInstanceCreator instanceCreator)
        {
            _instanceCreator = instanceCreator;

            Initialize();
        }

#if UNITY_EDITOR
        [Button]
        private void OnValidate()
        {
            uiElements = new List<DeckUIElement>();
            foreach (var element in Resources.FindObjectsOfTypeAll(typeof(DeckUIElement)))
            {
                var temp = element as DeckUIElement;
                uiElements.Add(temp);
            }
        }
#endif

        private void Initialize()
        {
            _inPoolElements = new Dictionary<DeckId, Stack<DeckUIElement>>();
            foreach (var deckUIElement in uiElements)
            {
                _inPoolElements[deckUIElement.PrefabId] = new Stack<DeckUIElement>();
            }

            _inPoolElementsTypeBased = new Dictionary<Type, DeckId>();

            foreach (var deckUIElement in uiElements)
            {
                _inPoolElementsTypeBased[deckUIElement.GetType()] = deckUIElement.PrefabId;
            }
        }

        public DeckUIElement Rent(DeckId prefabId)
        {
            if (_inPoolElements[prefabId].TryPop(out var poppedElement))
            {
                return poppedElement;
            }

            var newElement = _instanceCreator.CreateNewUIElement(prefabId.ID);
            newElement.Spawned();
            return newElement;
        }

        public T Rent<T>(DeckId prefabId) where T : DeckUIElement
        {
            if (_inPoolElements[prefabId].TryPop(out var poppedElement))
            {
                return poppedElement as T;
            }

            var newElement = _instanceCreator.CreateNewUIElement(prefabId.ID);
            newElement.Spawned();
            return newElement as T;
        }

        public T Rent<T>() where T : DeckUIElement
        {
            var prefabId = _inPoolElementsTypeBased[typeof(T)];
            return Rent<T>(prefabId);
        }

        public void Return(DeckUIElement element)
        {
            element.Despawned();
            _inPoolElements[element.PrefabId].Push(element);
        }


        public void Return<T>(List<T> elements) where T : DeckUIElement
        {
            if (elements.Count == 0)
            {
                return;
            }

            var prefabId = elements[0].PrefabId;
            var stack = _inPoolElements[prefabId];

            foreach (var deckUIElement in elements)
            {
                deckUIElement.Despawned();
                stack.Push(deckUIElement);
            }
        }
    }
}