using UnityEngine;
using Zenject;

namespace Deck.Data
{
    [CreateAssetMenu(menuName = "Deck/Binder/Agent", fileName = "Deck Binder Agent")]
    public class DeckBinderAgent : ScriptableObjectInstaller
    {
        [SerializeField] private DeckDataAgentCore coreDataAgentCore;
        [SerializeField] private DeckDataAgentChest chestAgentData;


        public override void InstallBindings()
        {
            Container.BindInstance(coreDataAgentCore);
            Container.BindInstance(chestAgentData);
        }
    }
}