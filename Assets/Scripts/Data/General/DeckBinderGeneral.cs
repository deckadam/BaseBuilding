using System;
using Deck.Animators;
using UnityEngine;
using Zenject;

namespace Deck.Data.General
{
    [CreateAssetMenu(menuName = "Deck/Binder/General", fileName = "Deck Binder General")]
    public class DeckBinderGeneral : ScriptableObjectInstaller
    {
        [SerializeField] private DeckAgentCore agentCorePrefab;
        [SerializeField] private DeckAgentChest agentChestPrefab;
        [SerializeField] private DeckGeneralData deckGeneralData;

        public override void InstallBindings()
        {
            Container.BindInstance(agentCorePrefab);
            Container.BindInstance(agentChestPrefab);
            Container.BindInstance(deckGeneralData);
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