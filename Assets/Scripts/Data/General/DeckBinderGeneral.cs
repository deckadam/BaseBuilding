using Deck.Agent;
using UnityEngine;
using Zenject;

namespace Deck.Data.General
{
    [CreateAssetMenu(menuName = "Deck/Binder/General", fileName = "Deck Binder General")]
    public class DeckBinderGeneral : ScriptableObjectInstaller
    {
        public DeckCoreAgent coreAgentPrefab;

        public override void InstallBindings()
        {
            Container.BindInstance(this);
        }
    }
}