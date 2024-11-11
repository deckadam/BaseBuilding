using System;
using Deck.Base;
using Deck.Base.Id;
using Deck.Save;
using Deck.Utility;
using UnityEngine;
using Zenject;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Deck.Components
{
    public class DeckUIElement : DeckPoolable
    {
        [SerializeField] private DeckId prefabId;

        public RectTransform rectTransform;

        protected DeckInstanceProvider InstanceProvider;

        private Transform _parent;
        private bool _isSpawned;

        public DeckId PrefabId
        {
            get
            {
                if (prefabId.IsValid)
                {
                    return prefabId;
                }

                throw new Exception("No valid prefab id " + name + " " + prefabId.ID);
            }
        }

        [Inject]
        private void Inject(DeckInstanceProvider instanceProvider)
        {
            InstanceProvider = instanceProvider;
        }

        private void OnValidate()
        {
            InternalOnValidate();
            rectTransform ??= GetComponent<RectTransform>();

#if UNITY_EDITOR
            if (!PrefabUtility.IsPartOfPrefabAsset(gameObject))
            {
                return;
            }
#endif

            foreach (var itemVisual in Resources.FindObjectsOfTypeAll(typeof(DeckUIElement)))
            {
                var uiElement = itemVisual as DeckUIElement;

                if (uiElement.name == name)
                {
                    continue;
                }

                if (uiElement.prefabId.Equals(prefabId))
                {
                    DeckLogger.Error("Multiple prefab id " + prefabId.ID + "  " + uiElement.prefabId.ID + "  " + name + "   " + uiElement.name);
                }
            }

            if (int.TryParse(name[^1].ToString(), out var _))
            {
                prefabId.ResetId();
                return;
            }

            if (!prefabId.IsValid)
            {
                prefabId = DeckId.CreateNew();
            }
        }

        public override void OnDespawned()
        {
            if (!_isSpawned)
            {
                DeckLogger.Warning("Trying to despawn already despawned object");
                return;
            }

            _isSpawned = false;
            transform.SetParent(_parent);
            gameObject.SetActive(false);
            InternalOnDespawned();
        }


        public override void OnSpawned()
        {
            if (_isSpawned)
            {
                DeckLogger.Warning("Trying to spawn already spawned object");
                return;
            }

            _parent = transform.parent;
            gameObject.SetActive(true);
            _isSpawned = true;

            InternalOnSpawned();
        }

        protected virtual void InternalOnValidate()
        {
        }

        protected virtual void InternalOnSpawned()
        {
        }

        protected virtual void InternalOnDespawned()
        {
        }
    }
}