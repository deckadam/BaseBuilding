using System;
using System.Collections.Generic;
using Deck.Data.Pool;
using Deck.Utility.Logger;
using UnityEngine;
using Zenject;

namespace Deck.Services.Implementations.ObjectPooling
{
    public class DeckPoolingService : DeckServiceBase
    {
        private Transform _defaultPoolParent;
        private const int POOL_SIZE = 5;
        private Dictionary<Type, IDeckPoolable> _bakedPoolables;
        private Dictionary<Type, List<IDeckPoolable>> _poolables;

        private DiContainer _container;
        private DeckPool _pool;

        [Inject]
        private void Inject(DiContainer container, DeckPool pool)
        {
            _container = container;
            _pool = pool;
            ProcessPoolables();
        }

        private void ProcessPoolables()
        {
            var poolParent = new GameObject();
            poolParent.name = "Pool Parent";

            _defaultPoolParent = poolParent.transform;

            _bakedPoolables = new Dictionary<Type, IDeckPoolable>();
            _poolables = new Dictionary<Type, List<IDeckPoolable>>();

            foreach (var poolable in _pool.poolables)
            {
                var typeOfPoolable = poolable.GetComponent<IDeckPoolable>().GetType();
                var poolablePrefab = poolable.GetComponent<IDeckPoolable>();
                _bakedPoolables[typeOfPoolable] = poolablePrefab;
                _poolables[typeOfPoolable] = new List<IDeckPoolable>();
                _poolables[typeOfPoolable].AddRange(InstantiatePoolables(poolablePrefab));
                DeckLogger.Service("Creating pool for " + typeOfPoolable);
            }
        }

        private IEnumerable<IDeckPoolable> InstantiatePoolables<T>(T prefab) where T : IDeckPoolable
        {
            var result = new IDeckPoolable[POOL_SIZE];
            for (var i = 0; i < POOL_SIZE; i++)
            {
                var newPoolable = _container.InstantiatePrefab(prefab.getMonoBehaviour(), _defaultPoolParent).GetComponent<IDeckPoolable>();
                newPoolable.Initialize();
                newPoolable.getMonoBehaviour().gameObject.SetActive(false);
                result[i] = newPoolable;
            }

            return result;
        }

        public T GetFromPool<T>() where T : IDeckPoolable
        {
            var typeOfPoolable = typeof(T);

            if (!_poolables.ContainsKey(typeOfPoolable))
            {
                DeckLogger.Service("Pool doesn't have this type of object initialized  " + typeOfPoolable + "   " + typeof(T));
                return default;
            }

            var pool = _poolables[typeOfPoolable];

            if (pool.Count == 0)
            {
                ExpandPoolForSpecifiedType<T>(typeOfPoolable);
            }

            var pooledObject = RetrieveFirstObjectFromPool<T>(pool);
            return pooledObject.getMonoBehaviour().GetComponent<T>();
        }

        public void ReturnToPool<T>(T objectToReturn) where T : IDeckPoolable
        {
            objectToReturn.getMonoBehaviour().transform.SetParent(_defaultPoolParent);
            _poolables[typeof(T)].Add(objectToReturn);
        }

        private IDeckPoolable RetrieveFirstObjectFromPool<T>(List<IDeckPoolable> pool) where T : IDeckPoolable
        {
            var pooledObject = pool[0];
            pool.RemoveAt(0);
            return pooledObject;
        }

        private void ExpandPoolForSpecifiedType<T>(Type typeOfPoolable) where T : IDeckPoolable
        {
            DeckLogger.Service("Pool is empty for type creating new ones  " + typeOfPoolable);
            var prefab = _bakedPoolables[typeof(T)];
            Debug.LogError(prefab == null);
            _poolables[typeOfPoolable].AddRange(InstantiatePoolables(prefab));
        }
    }
}