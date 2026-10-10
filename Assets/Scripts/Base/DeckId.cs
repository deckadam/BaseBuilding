using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Base
{
    [Serializable]
    public struct DeckId
    {
#if UNITY_EDITOR
        public string StringId => _id.ToString();
#endif

        [SerializeField, HideInInspector] private int _id;

        public string IdString => _id.ToString();
        public bool IsValid => _id != 0;
        public int Id => _id;

        private DeckId(bool justShutUp = false)
        {
            _id = Random.Range(10000000, 99999999);
        }

        public DeckId(string id)
        {
            if (!int.TryParse(id, out var intId))
            {
                throw new Exception($"Bullshit input {id}");
            }

            _id = intId;
        }

        public DeckId(int id)
        {
            _id = id;
        }

        public static DeckId CreateNew()
        {
            return new DeckId(false);
        }

        public void ResetId(bool force = false)
        {
            if (_id == 0 || force)
            {
                _id = Random.Range(10000000, 99999999);
            }
        }

        public bool Equals(DeckId other)
        {
            return _id == other._id;
        }

        public override int GetHashCode()
        {
            return _id;
        }

        public static explicit operator string(DeckId id) => id.ToString();
        public static explicit operator int(DeckId id) => id.Id;
    }
}