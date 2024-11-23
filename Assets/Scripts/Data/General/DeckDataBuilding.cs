using UnityEngine;
using Zenject;

namespace Deck.Data.General
{
    [CreateAssetMenu(menuName = "Deck/Binder/Building", fileName = "Deck Building Data")]
    public class DeckDataBuilding : ScriptableObjectInstaller
    {
        [SerializeField] private float buildableRotationSpeed;
        [SerializeField] private Material availableMaterial;
        [SerializeField] private Material unavailableMaterial;
        [SerializeField] private Mesh accessCellMesh;

        public override void InstallBindings()
        {
            Container.BindInstance(this);
        }

        public Material GetAvailableMaterial() => availableMaterial;
        public Material GetUnavailableMaterial() => unavailableMaterial;
        public float GetBuildableRotationSpeed() => buildableRotationSpeed;
        public Mesh GetAccessCellMesh() => accessCellMesh;
    }
}