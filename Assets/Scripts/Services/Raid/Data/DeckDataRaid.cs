using System;
using UnityEngine;

namespace Services.Raid.Data
{
    [Serializable]
    public class DeckDataRaid
    {
        [SerializeField] private int raiderCount;

        public int RaiderCount => raiderCount;
    }
}