using System;
using Base;
using UnityEngine;

namespace ItemVisualProviders.Wall.SubProviders
{
    [Serializable]
    public class DeckItemVisualProviderWallBasic
    {
        [SerializeField] private DeckItemVisual wallItemVisual;

        protected DeckItemVisualProviderWall _wallProvider;

        public virtual int NeighbourSetSize => 0;

        public void Initialize(DeckItemVisualProviderWall wallProvider)
        {
            _wallProvider = wallProvider;
            InternalInitialize();
        }

        protected virtual void InternalInitialize()
        {
        }

        public virtual bool IsSupportedItemVisual(DeckId prefabId)
        {
            return wallItemVisual.PrefabId.Equals(prefabId);
        }

        public virtual bool IsSupportedItemVisual(DeckItemVisual itemVisual)
        {
            return wallItemVisual.PrefabId.Equals(itemVisual.PrefabId);
        }

        public virtual DeckItemVisual GetVisualToPlace(Vector2Int position, ref bool[] checkList)
        {
            return wallItemVisual;
        }
    }
}