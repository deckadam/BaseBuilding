using Deck.InGame.Agent.Waiter;
using Deck.Instancing;
using Deck.ItemVisualProviders;
using Deck.Save;
using Deck.Services.Building;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Deck.Data.General
{
    [CreateAssetMenu(menuName = "Deck/Binder/General", fileName = "Deck Binder General")]
    public class DeckBinderGeneral : ScriptableObjectInstaller
    {
        [SerializeField] private DeckItemVisualProviderBasic[] itemVisualProviders;
        [SerializeField] private DeckItemVisualProviderWall wallProvider;
        [SerializeField] private DeckItemVisualProviderDoor doorProvider;

        [FormerlySerializedAs("agentCorePrefab")] [SerializeField] private DeckAgentWaiter agentWaiterPrefab;
        [SerializeField] private DeckInstanceProvider instanceProvider;

        public override void InstallBindings()
        {
            Container.Bind<DeckLoadResolver>().AsSingle();
            Container.Bind<DeckSilhouetteProvider>().AsSingle();

            Container.BindInstance(agentWaiterPrefab);
            Container.BindInstance(itemVisualProviders);
            Container.BindInstance(wallProvider);
            Container.BindInstance(doorProvider);
            Container.BindInstance(instanceProvider);

            foreach (var deckItemVisualProviderBasic in itemVisualProviders)
            {
                Container.QueueForInject(deckItemVisualProviderBasic);
            }

            Container.QueueForInject(instanceProvider);
        }
    }
}