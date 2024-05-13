using System;
using Deck.Agent;
using Deck.Agent.Chest;
using Deck.Animators;
using Deck.ItemVisualProviders;
using UnityEngine;
using Zenject;

namespace Deck.Data.General
{
    [CreateAssetMenu(menuName = "Deck/Binder/General", fileName = "Deck Binder General")]
    public class DeckBinderGeneral : ScriptableObjectInstaller
    {
        [SerializeField] private DeckItemVisualProviderBasic[] itemVisualProviders;
        [SerializeField] private DeckAgentCore agentCorePrefab;
        [SerializeField] private DeckAgentChest agentChestPrefab;
        [SerializeField] private DeckGeneralData deckGeneralData;

        public override void InstallBindings()
        {
            Container.BindInstance(agentCorePrefab);
            Container.BindInstance(agentChestPrefab);
            Container.BindInstance(deckGeneralData);
            Container.BindInstance(itemVisualProviders);
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