using UnityEngine;
using Zenject;

namespace Deck.Data.Map
{
    [CreateAssetMenu(menuName = "Deck/Installer/Map", fileName = "Deck Map Data")]
    public class DeckMapData : ScriptableObjectInstaller
    {
        public Vector2Int size;
        public GameObject blockade;
        public float noiseScale;
        public float blockadeThreshhold;
        public Material basePlaneMaterial;

        public override void InstallBindings()
        {
            Container.BindInstance(this);
        }
    }
}