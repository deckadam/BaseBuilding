using System;
using Deck.UI.InGame;
using Deck.UI.Pool;
using Deck.Utility.Logger;
using UnityEngine;
using Zenject;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Deck.Utility.Poolable
{
    public class DeckUIElement : MonoBehaviour
    {
        [SerializeField] private DeckId prefabId;

        public RectTransform rectTransform;

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

        protected DeckUIPool uiPool;

        [Inject]
        private void Inject(DeckUIPool uiPool)
        {
            this.uiPool = uiPool;
        }


        private void OnValidate()
        {
            InternalOnValidate();
            rectTransform ??= GetComponent<RectTransform>();

#if UNITY_EDITOR
            if (PrefabUtility.GetPrefabParent(gameObject) == null && !PrefabUtility.IsPartOfPrefabAsset(gameObject))
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
                    DeckLogger.Error("Multiple prefab id " + prefabId.ID + "  " + uiElement.prefabId.ID+"  " + name +"   "+ uiElement.name);
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

        protected virtual void InternalOnValidate()
        {
            
        }

        public void Despawned()
        {
            if (!_isSpawned)
            {
                DeckLogger.Warning("Trying to despawn already despawned object");
                return;
            }

            _isSpawned = false;
            transform.SetParent(_parent);
            gameObject.SetActive(false);
            OnDespawned();
        }

        public void Spawned()
        {
            if (_isSpawned)
            {
                DeckLogger.Warning("Trying to spawn already spawned object");
                return;
            }

            _parent = transform.parent;
            gameObject.SetActive(true);
            _isSpawned = true;

            OnSpawned();
        }

        protected virtual void OnSpawned()
        {
        }

        protected virtual void OnDespawned()
        {
        }
    }
}