using Data.Camera;
using GameManager.Data.GameSetting;
using InGame.Agent.Raid;
using InGame.Agent.Waiter;
using Instancing;
using ItemVisualProviders;
using ItemVisualProviders.Door;
using ItemVisualProviders.Wall;
using Services.Building;
using Services.Building.Buildable;
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
        [SerializeField] private DeckGameSettingBasic[] gameSettings;
        [SerializeField] private DeckCameraParameters cameraParameters;
        [SerializeField] private DeckBuildable[] allBuildables;
        [SerializeField] private DeckAgentRaider[] allRaiders;

        public override void InstallBindings()
        {
            Container.Bind<DeckLoadResolver>().AsSingle();
            Container.Bind<DeckSilhouetteProvider>().AsSingle();

            Container.BindInstance(agentWaiterPrefab);
            Container.BindInstance(itemVisualProviders);
            Container.BindInstance(wallProvider);
            Container.BindInstance(doorProvider);
            Container.BindInstance(instanceProvider);
            Container.BindInstance(cameraParameters);
            Container.BindInstance(gameSettings);
            Container.BindInstance(allBuildables);
            Container.BindInstance(allRaiders);

            foreach (var deckBuildable in allBuildables)
            {
                deckBuildable.Initialize();
            }

            foreach (var deckItemVisualProviderBasic in itemVisualProviders)
            {
                Container.QueueForInject(deckItemVisualProviderBasic);
            }

            Container.QueueForInject(instanceProvider);
        }
    }
}