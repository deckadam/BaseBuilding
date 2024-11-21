using System;
using UnityEngine;

namespace Deck.Services.Building
{
    [Serializable]
    public struct DeckSilhouetteData
    {
        [SerializeField] private Mesh mesh;
        [SerializeField] private Vector3 position;
        [SerializeField] private Vector3 rotation;

        public DeckSilhouetteData(Mesh mesh, Vector3 position, Vector3 rotation)
        {
            this.mesh = mesh;
            this.position = position;
            this.rotation = rotation;
        }

        public Mesh GetMesh() => mesh;
        public Vector3 GetPosition() => position;
        public Vector3 GetRotation() => rotation;
    }
}