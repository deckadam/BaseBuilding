#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using Cysharp.Threading.Tasks;
using Deck.Base;
using Deck.Data.Item;
using Deck.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Deck.Base
{
    public class DeckItemVisual : DeckPoolable
    {
        [Unity.Collections.ReadOnly, SerializeField]
        private bool isStatic;

        [SerializeField] private Vector3 localEquipRotation;
        [SerializeField] private Vector3 localEquipPosition;
        [SerializeField] private Vector3 displayOffset;
        [SerializeField] private new Rigidbody rigidbody;
        [SerializeField] private new Collider collider;
        [SerializeField] private float size;
        [SerializeField] private DeckId uniqueId;
        [SerializeField] private DeckId prefabId;
        [SerializeField] private bool canBePlacedOnTop;

        public Vector3 LocalEquipPosition => localEquipPosition;
        public Vector3 LocalEquipRotation => localEquipRotation;
        public DeckId UniqueId => uniqueId;
        public Collider Collider => collider;
        public bool CanBePlacedOnTop => canBePlacedOnTop;
        private bool _hasInitialized;

        private DeckAgent _agent;

        public DeckId PrefabId
        {
            get
            {
                if (prefabId.IsValid)
                {
                    return prefabId;
                }

                throw new Exception("No valid prefab id " + name);
            }
        }


        public void SetNewUniqueId()
        {
            uniqueId = DeckId.CreateNew();
        }

        public void SetUniqueId(DeckId id)
        {
            uniqueId = id;
        }

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (_hasInitialized)
            {
                return;
            }

            _hasInitialized = true;
        }

        [Button]
        private void CalculateSize()
        {
            if (collider != null)
            {
                if (collider is BoxCollider boxCollider)
                {
                    var sizes = boxCollider.bounds.size;
                    size = Mathf.Max(sizes.x, sizes.z) / 2f;
                    Debug.LogError(sizes);
                }
                else if (collider is SphereCollider sphereCollider)
                {
                    size = sphereCollider.radius / 2f;
                    Debug.LogError(size);
                }
                else if (collider is CapsuleCollider capsuleCollider)
                {
                    var sizes = capsuleCollider.bounds.size;
                    size = Mathf.Max(sizes.x, sizes.z) / 2f;
                    Debug.LogError(sizes);
                }
                else
                {
                    DeckLogger.Error($"Invalid collider type {collider.GetType()}");
                }
            }
        }

        [Button]
        public void OnValidate()
        {
            rigidbody = GetComponent<Rigidbody>();
            isStatic = rigidbody == null;

#if UNITY_EDITOR
            if (!PrefabUtility.IsPartOfPrefabAsset(gameObject))
            {
                return;
            }
#endif

            foreach (var itemVisual in Resources.FindObjectsOfTypeAll(typeof(DeckItemVisual)))
            {
                var itemVisualComponent = itemVisual as DeckItemVisual;

                if (itemVisualComponent.name == name)
                {
                    continue;
                }

                if (itemVisualComponent.prefabId.Equals(prefabId))
                {
                    DeckLogger.Error("Multiple prefab id " + prefabId.ID + " " + name + itemVisualComponent.name);
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

        public void SetAgent(DeckAgent agent)
        {
            _agent = agent;
        }

        public DeckAgent GetAgent()
        {
            return _agent;
        }

        public void OnEquip()
        {
            if (!isStatic)
            {
                rigidbody.isKinematic = true;
            }
        }

        public void ThrowInRandomDirection(float forceMultiplier = 10f)
        {
            if (isStatic) return;

            collider.enabled = true;

            var force = Random.insideUnitSphere;
            force.y = 0.5f;
            force = force.normalized * forceMultiplier;

            rigidbody.isKinematic = false;
            rigidbody.AddForce(force, ForceMode.Impulse);
        }

        public string GetAdditionalData()
        {
            return null;
        }

        public float GetSize()
        {
            return size;
        }
    }
}