using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Deck.UI.InGame
{
    [Serializable]
    public class DeckId
    {
        [ReadOnly] private Guid _id;


#if UNITY_EDITOR
        [ShowInInspector] public string StringId => _id.ToString();
#endif
        public Guid ID => _id;

        public DeckId()
        {
            _id = Guid.NewGuid();
        }

        public DeckId(Guid id)
        {
            _id = id;
        }

        public DeckId(string id)
        {
            Debug.LogError(id);
            _id = Guid.Parse(id);
        }

        public static DeckId CreateNew()
        {
            return new DeckId(Guid.NewGuid());
        }

        [Button]
        public void ResetId(bool force = false)
        {
            if (_id == Guid.Empty || force)
            {
                _id = Guid.NewGuid();
            }
        }
    }
}