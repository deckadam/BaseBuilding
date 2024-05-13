using UnityEngine;
using Zenject;

namespace Deck.Utility.Poolable
{
    public class DeckPoolable : MonoBehaviour, IPoolable<IMemoryPool>
    {
        private IMemoryPool _pool;
        private Transform _parent;

        protected virtual void Spawned()
        {
        }

        protected virtual void Despawned()
        {
        }

        public void Despawn()
        {
            gameObject.SetActive(false);
            _pool.Despawn(this);
        }

        public void OnDespawned()
        {
            Despawned();
            transform.SetParent(_parent);
        }

        public void OnSpawned(IMemoryPool p1)
        {
            _pool = p1;
            _parent = transform.parent;
            gameObject.SetActive(true);
            Spawned();
        }
    }
}