using UnityEngine;
using Zenject;

namespace Deck.Data.General
{
    [CreateAssetMenu(menuName = "Deck/Binder/Building", fileName = "Deck Building Data")]
    public class DeckDataBuilding : ScriptableObjectInstaller
    {
        [SerializeField] private Material availableMaterial;
        [SerializeField] private Material unavailableMaterial;

        public override void InstallBindings()
        {
            Container.BindInstance(this);
        }

        public Material GetAvailableMaterial() => availableMaterial;
        public Material GetUnavailableMaterial() => unavailableMaterial;
    }
}