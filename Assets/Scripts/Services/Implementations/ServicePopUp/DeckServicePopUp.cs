using System;
using System.Collections.Generic;
using Deck.Components.Building;
using Deck.Save;
using Deck.Utility.Logger;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Deck.Services
{
    public class DeckServicePopUp : DeckServiceBase
    {
        [SerializeField] private List<DeckPopUpBase> popUps;

        private Dictionary<Type, DeckPopUpBase> _typeDictionary;
        private DeckInstanceProvider _instanceProvider;

#if UNITY_EDITOR
        [Button]
        private void OnValidate()
        {
            popUps = new List<DeckPopUpBase>();
            foreach (var popup in Resources.FindObjectsOfTypeAll(typeof(DeckPopUpBase)))
            {
                if (!PrefabUtility.IsPartOfPrefabAsset(popup))
                {
                    continue;
                }

                var element = popup as DeckPopUpBase;

                if (!string.IsNullOrEmpty(element.gameObject.scene.name))
                {
                    continue;
                }

                popUps.Add(popup as DeckPopUpBase);
            }
        }
#endif

        [Inject]
        private void Inject(DeckInstanceProvider instanceProvider)
        {
            _instanceProvider = instanceProvider;
        }

        public override void Initialize()
        {
            _typeDictionary = new Dictionary<Type, DeckPopUpBase>();
            foreach (var deckPopUpBase in popUps)
            {
                _typeDictionary[deckPopUpBase.GetType()] = deckPopUpBase;
            }
        }

        public T OpenPopUp<T>() where T : DeckPopUpBase
        {
            if (!_typeDictionary.TryGetValue(typeof(T), out var popUp))
            {
                DeckLogger.Error("Prefab not found type: " + typeof(T).Name);
            }

            return _instanceProvider.RentUIElement<T>().GetComponent<T>();
        }
    }
}