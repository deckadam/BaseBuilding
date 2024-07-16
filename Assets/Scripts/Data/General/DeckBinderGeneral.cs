using System;
using Deck.Agent;
using Deck.Agent.Chest;
using Deck.Animators;
using Deck.ItemVisualProviders;
using Deck.UI.Pool;
using UnityEngine;
using Zenject;

namespace Deck.Data.General
{
    [CreateAssetMenu(menuName = "Deck/Binder/General", fileName = "Deck Binder General")]
    public class DeckBinderGeneral : ScriptableObjectInstaller
    {
        [SerializeField] private DeckItemVisualProviderBasic[] itemVisualProviders;
        [SerializeField] private DeckItemVisualProviderWall wallProvider;
        [SerializeField] private DeckUIPool uiPool;

        [SerializeField] private DeckAgentCore agentCorePrefab;
        [SerializeField] private DeckAgentChest agentChestPrefab;
        [SerializeField] private DeckGeneralData deckGeneralData;

        public override void InstallBindings()
        {
            Container.BindInstance(agentCorePrefab);
            Container.BindInstance(agentChestPrefab);
            Container.BindInstance(deckGeneralData);
            Container.BindInstance(itemVisualProviders);
            Container.BindInstance(wallProvider);
            Container.BindInstance(uiPool);

            Container.QueueForInject(uiPool);

            foreach (var deckItemVisualProviderBasic in itemVisualProviders)
            {
                Container.QueueForInject(deckItemVisualProviderBasic);
            }
        }

        [Serializable]
        public class DeckGeneralData
        {
            [SerializeField] private DeckAnimationParametersAnimationCurve itemCollectingFlyAnimation;
            [SerializeField] private Material atlasMaterial;

            public DeckAnimationParametersAnimationCurve ItemCollectingFlyAnimation => itemCollectingFlyAnimation;
            public Material AtlasMaterial => atlasMaterial;
        }
    }
}