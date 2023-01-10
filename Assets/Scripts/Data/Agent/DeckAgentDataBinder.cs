using Deck.Player;
using UnityEngine;
using Zenject;

namespace Deck.Test.Data.Agent
{
    [CreateAssetMenu(menuName = "Deck/Installer/AgentDataBinder", fileName = "Deck Agent Data Binder")]
    public class DeckAgentDataBinder : ScriptableObjectInstaller
    {
        [SerializeField] private DeckAgentData coreAgentData;

        public override void InstallBindings()
        {
            Container.BindInstance(coreAgentData).WhenInjectedInto<DeckCoreAgent>();
        }
    }
}