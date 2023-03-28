using UnityEngine;
using Zenject;

namespace Deck.Data.Map
{
    [CreateAssetMenu(menuName = "Deck/Binder/Map", fileName = "Deck Binder Map")]
    public class DeckBinderMap : ScriptableObjectInstaller
    {
        [SerializeField] private Vector2Int size;
        [SerializeField] private Material basePlaneMaterial;

        public Vector2Int GetSize() => size;
        public Material GetBasePlaneMaterial() => basePlaneMaterial;

        public override void InstallBindings()
        {
            Container.BindInstance(this);
        }
    }
}