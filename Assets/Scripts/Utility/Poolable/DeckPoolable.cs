using Deck.Utility.Logger;
using UnityEngine;
using Zenject;

namespace Deck.Utility.Poolable
{
    public class DeckPoolable : MonoBehaviour, IPoolable<IMemoryPool>
    {
        private IMemoryPool _pool;
        private Transform _parent;
        private bool _isSpawned;
        protected virtual void Spawned()
        {
        }

        protected virtual void Despawned()
        {
        }

        public void Despawn()
        {
            if (!_isSpawned)
            {
                DeckLogger.Warning("Trying to despawn already despawned object");
                return;
            }
            
            gameObject.SetActive(false);
            _pool.Despawn(this);
            _isSpawned = false;
        }

        public void OnDespawned()
        {
            Despawned();
            transform.SetParent(_parent);
        }

        public void OnSpawned(IMemoryPool p1)
        {
            if (_isSpawned)
            {
                DeckLogger.Warning("Trying to spawn already spawned object");
                return;
            }
            
            _pool = p1;
            _parent = transform.parent;
            gameObject.SetActive(true);
            Spawned();
            _isSpawned = true;
        }
    }
}