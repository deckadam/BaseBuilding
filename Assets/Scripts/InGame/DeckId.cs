using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Deck.UI.InGame
{
    [Serializable]
    public struct DeckId
    {
#if UNITY_EDITOR
        [ShowInInspector] public string StringId => _id.ToString();
#endif

        [SerializeField, HideInInspector] private int _id;

        public string IdString;
        public bool IsValid => _id != 0;
        public int ID => _id;


        private DeckId(bool justShupUp = false)
        {
            _id = Random.Range(10000000, 99999999);
            IdString = _id.ToString();
        }


        public DeckId(string id)
        {
            if (!int.TryParse(id, out var intId))
            {
                throw new Exception($"Invalid id {id}");
            }

            _id = intId;
            IdString = intId.ToString();
        }

        public DeckId(int id)
        {
            _id = id;
            IdString = _id.ToString();
        }

        public static DeckId CreateNew()
        {
            return new DeckId(false);
        }

        [Button]
        public void ResetId(bool force = false)
        {
            if (_id == 0 || force)
            {
                _id = Random.Range(10000000, 99999999);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is not DeckId deckId)
                return false;

            return deckId._id == _id;
        }

        public bool Equals(DeckId other)
        {
            return _id == other._id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_id, IdString);
        }

        public static explicit operator string(DeckId id) => id.ToString();
    }
}