using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Deck;
using Deck.Components;
using Deck.Data.Item;
using Deck.Data.Map;
using Deck.MVC;
using Deck.Save;
using Deck.Save.Data;
using Deck.Services;
using Deck.Events.CameraService;
using Deck.Events.MapService;
using Deck.Events.Navigation;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Deck.UI
{
    public class DeckGameManager : DeckServiceBase
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitializeGame()
        {
            DeckMVC<DeckDataItem, IEnumerable<DeckDataItem>>.ResetController();
            DeckMVC<DeckComponentHealth, IEnumerable<DeckComponentHealth>>.ResetController();
        }

        private DiContainer _container;
        private DeckAgentLoadResolver _loadResolver;
        private DeckAgentCore _coreAgentPrefab;

        [SerializeField] private DeckBinderMap binderMap;

        [Inject]
        private void Inject(DiContainer container, DeckAgentLoadResolver loadResolver, DeckAgentCore agentCorePrefab)
        {
            _container = container;
            _loadResolver = loadResolver;
            _coreAgentPrefab = agentCorePrefab;
        }

        [Button]
        public void CreateNewGameButton()
        {
            FindObjectOfType<DeckServiceMap>().CreateGround(binderMap);
            FindObjectOfType<DeckServiceNavigation>().GenerateNavigation(binderMap);
            FindObjectOfType<DeckServiceCamera>().GenerateCameraBounds(binderMap);
        }

        public async void CreateNewGame()
        {
            await global::Deck.Deck.GetService<DeckServiceMap>().LoadMap();
            await UniTask.Delay(1000);
            _container.InstantiatePrefab(_coreAgentPrefab).GetComponent<DeckAgentCore>().StartWithClearData();
        }

        public async void LoadGame()
        {
            await global::Deck.Deck.GetService<DeckServiceMap>().LoadMap();
            global::Deck.Deck.GetService<DeckServiceMap>().CreateGround(binderMap);
            global::Deck.Deck.GetService<DeckServiceNavigation>().GenerateNavigation(binderMap);
            global::Deck.Deck.GetService<DeckServiceCamera>().GenerateCameraBounds(binderMap);

            var playerData = DeckSaveSystem.GetData<DeckComponentHolderSaveDatas>(nameof(DeckComponentHolderSaveDatas));
            _loadResolver.ResolveAndLoad(playerData);
        }

        public void Test_FillSaveFile()
        {
            var newCoreAgentData = new DeckComponentHolderSaveDatas();
            foreach (var deckCoreAgent in FindObjectsOfType<DeckAgent>())
            {
                if (!deckCoreAgent.WillSave())
                {
                    continue;
                }

                newCoreAgentData.datas.Add(new DeckComponentHolderSaveData
                {
                    id = deckCoreAgent.GetId(),
                    componentDatas = deckCoreAgent.GetSaveData()
                });
            }

            DeckSaveSystem.SetData(nameof(DeckComponentHolderSaveDatas), newCoreAgentData);
        }
    }
}