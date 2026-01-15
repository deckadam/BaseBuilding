using InGame.Agent.Waiter;
using Instancing;
using ItemVisualProviders;
using Services.Building;
using Systems.SystemSave;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Data.General
{
    [CreateAssetMenu(menuName = "Deck/Binder/General", fileName = "Deck Binder General")]
    public class DeckBinderGeneral : ScriptableObjectInstaller
    {
        [SerializeField] private DeckItemVisualProviderBasic[] itemVisualProviders;
        [SerializeField] private DeckItemVisualProviderWall wallProvider;
        [SerializeField] private DeckItemVisualProviderDoor doorProvider;

        [FormerlySerializedAs("agentCorePrefab")] [SerializeField]
        private DeckAgentWaiter agentWaiterPrefab;

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