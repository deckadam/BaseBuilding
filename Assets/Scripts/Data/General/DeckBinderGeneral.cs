using InGame.Agent.Waiter;
using InGame.Map.Data;
using Instancing;
using ItemVisualProviders;
using ItemVisualProviders.Door;
using ItemVisualProviders.Wall;
using Services.Building;
using Systems.SystemSave;
using UnityEngine;
using Zenject;

namespace Data.General
{
    [CreateAssetMenu(menuName = "Deck/Binder/General", fileName = "Deck Binder General")]
    public class DeckBinderGeneral : ScriptableObjectInstaller
    {
        [SerializeField] private DeckItemVisualProviderBasic[] itemVisualProviders;
        [SerializeField] private DeckItemVisualProviderWall wallProvider;
        [SerializeField] private DeckItemVisualProviderDoor doorProvider;
        [SerializeField] private DeckAgentWaiter agentWaiterPrefab;
        [SerializeField] private DeckInstanceProvider instanceProvider;
        [SerializeField] private DeckDataMap mapData;

        public override void InstallBindings()
        {
            Container.Bind<DeckLoadResolver>().AsSingle();
            Container.Bind<DeckSilhouetteProvider>().AsSingle();

            Container.BindInstance(agentWaiterPrefab);
            Container.BindInstance(itemVisualProviders);
            Container.BindInstance(wallProvider);
            Container.BindInstance(doorProvider);
            Container.BindInstance(instanceProvider);
            Container.BindInstance(mapData);

            foreach (var deckItemVisualProviderBasic in itemVisualProviders)
            {
                Container.QueueForInject(deckItemVisualProviderBasic);
            }

            Container.QueueForInject(instanceProvider);
        }
    }
}