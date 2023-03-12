using Deck.Agent;
using Deck.Map.Agent.Chest;
using UnityEngine;
using Zenject;

namespace Deck.Data.General
{
    [CreateAssetMenu(menuName = "Deck/Binder/General", fileName = "Deck Binder General")]
    public class DeckBinderAgentFactory : ScriptableObjectInstaller
    {
        [SerializeField] private DeckAgentCore agentCorePrefab;
        [SerializeField] private DeckAgentChest agentChestPrefab;

        public override void InstallBindings()
        {
            Container.BindFactory<DeckAgentCore, DeckAgentCore.Factory>().FromComponentInNewPrefab(agentCorePrefab);
            Container.BindFactory<DeckAgentChest, DeckAgentChest.Factory>().FromComponentInNewPrefab(agentChestPrefab);
        }
    }
}