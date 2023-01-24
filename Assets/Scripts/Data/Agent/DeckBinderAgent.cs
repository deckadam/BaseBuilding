using Deck.Agent;
using UnityEngine;
using Zenject;

namespace Deck.Data.Agent
{
    [CreateAssetMenu(menuName = "Deck/Binder/Agent", fileName = "Deck Binder Agent")]
    public class DeckBinderAgent : ScriptableObjectInstaller
    {
        [SerializeField] private DeckDataAgent coreDataAgent;

        public override void InstallBindings()
        {
            Container.BindInstance(coreDataAgent).WhenInjectedInto<DeckCoreAgent>();
        }
    }
}