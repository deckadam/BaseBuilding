using System;
using Deck.Animators;
using Deck.InGame.Agent.Building.Pool;
using Deck.InGame.Agent.Core;
using Deck.ItemVisualProviders;
using UnityEngine;
using Zenject;

namespace Deck.Data.General
{
    [CreateAssetMenu(menuName = "Deck/Binder/General", fileName = "Deck Binder General")]
    public class DeckBinderGeneral : ScriptableObjectInstaller
    {
        [SerializeField] private DeckItemVisualProviderBasic[] itemVisualProviders;
        [SerializeField] private DeckItemVisualProviderWall wallProvider;
        [SerializeField] private DeckItemVisualProviderDoor doorProvider;
        [SerializeField] private DeckUIPool uiPool;

        [SerializeField] private DeckAgentCore agentCorePrefab;
        [SerializeField] private DeckGeneralData deckGeneralData;

        public override void InstallBindings()
        {
            Container.BindInstance(agentCorePrefab);
            Container.BindInstance(deckGeneralData);
            Container.BindInstance(itemVisualProviders);
            Container.BindInstance(wallProvider);
            Container.BindInstance(doorProvider);
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