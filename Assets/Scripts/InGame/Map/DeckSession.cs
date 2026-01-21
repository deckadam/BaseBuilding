using Cinemachine;
using UnityEngine;

namespace InGame.Map
{
    public class DeckSession : MonoBehaviour
    {
        [SerializeField] private Collider mapBoundCollider;

        private void Awake()
        {
            FindObjectOfType<CinemachineConfiner>().m_BoundingVolume = mapBoundCollider;
        }

        public Collider GetCameraCollider()
        {
            return mapBoundCollider;
        }
    }
}