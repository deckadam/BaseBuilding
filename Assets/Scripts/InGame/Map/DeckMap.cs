using Cinemachine;
using UnityEngine;

namespace Deck
{
    public class DeckMap : MonoBehaviour
    {
        [SerializeField] private Collider mapBoundCollider;

        private void Awake()
        {
            FindObjectOfType<CinemachineConfiner>().m_BoundingVolume = mapBoundCollider;
        }
    }
}