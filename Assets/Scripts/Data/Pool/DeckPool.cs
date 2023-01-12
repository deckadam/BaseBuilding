using Sirenix.Serialization;
using UnityEngine;
using Zenject;

namespace Deck.Data.Pool
{
    [CreateAssetMenu(fileName = "Deck Pool", menuName = "Deck/Installer/Pool", order = 0)]
    public class DeckPool : ScriptableObjectInstaller
    {
        [OdinSerialize] public GameObject[] poolables;

        public override void InstallBindings()
        {
            Container.BindInstance(this);
        }
    }
}