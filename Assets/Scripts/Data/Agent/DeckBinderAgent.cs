using Deck.Agent;
using Deck.Map.Agent.Chest;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Deck.Data.Agent
{
    [CreateAssetMenu(menuName = "Deck/Binder/Agent", fileName = "Deck Binder Agent")]
    public class DeckBinderAgent : ScriptableObjectInstaller
    {
        [SerializeField] private DeckDataAgentCore coreDataAgentCore;
        [SerializeField] private DeckDataAgentChest chestAgentData;


        public override void InstallBindings()
        {
            Container.BindInstance(coreDataAgentCore).WhenInjectedInto<DeckAgentCore>();
            Container.BindInstance(chestAgentData).WhenInjectedInto<DeckAgentChest>();
        }
    }
}