using Deck.Agent;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Deck.Data.General
{
    [CreateAssetMenu(menuName = "Deck/Binder/General", fileName = "Deck Binder General")]
    public class DeckBinderGeneral : ScriptableObjectInstaller
    {
        [FormerlySerializedAs("coreAgentPrefab")] public DeckAgentCore agentCorePrefab;

        public override void InstallBindings()
        {
            Container.BindInstance(this);
        }
    }
}