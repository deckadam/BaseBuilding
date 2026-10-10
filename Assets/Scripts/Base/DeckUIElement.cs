using System;
using Instancing;
using UnityEditor;
using UnityEngine;
using Utility;
using Zenject;

namespace Base
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

                throw new Exception("No valid prefab id " + name + " " + prefabId.Id);
            }
        }

        [Inject]
        private void Inject(DeckInstanceProvider instanceProvider)
        {
            InstanceProvider = instanceProvider;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            InternalOnValidate();
            rectTransform ??= GetComponent<RectTransform>();

            if (!PrefabUtility.IsPartOfPrefabAsset(gameObject))
            {
                return;
            }

            foreach (var itemVisual in Resources.FindObjectsOfTypeAll(typeof(DeckUIElement)))
            {
                var uiElement = itemVisual as DeckUIElement;

                if (uiElement == null)
                {
                    continue;
                }

                if (uiElement.GetHashCode() == GetHashCode())
                {
                    continue;
                }

                if (uiElement.GetEntityId() == GetEntityId())
                {
                    continue;
                }

                if (uiElement.prefabId.Equals(prefabId))
                {
                    DeckLogger.Error("Multiple prefab id " + prefabId.Id + "  " + uiElement.prefabId.Id + "  " + name + "   " + uiElement.name);
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
#endif

        public override void OnDeSpawned()
        {
            if (!_isSpawned)
            {
                DeckLogger.Warning("Trying to return to pool already pooled object");
                return;
            }

            _isSpawned = false;
            transform.SetParent(_parent);
            gameObject.SetActive(false);
            InternalOnDeSpawned();
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

        protected virtual void InternalOnDeSpawned()
        {
        }
    }
}