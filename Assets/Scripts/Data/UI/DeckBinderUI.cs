using System.Collections.Generic;
using UI.Building.Data;
using UnityEngine;
using Zenject;

namespace Data.UI
{
    [CreateAssetMenu(fileName = "Deck Binder UI", menuName = "Deck/Binder/UI", order = 0)]
    public class DeckBinderUI : ScriptableObjectInstaller
    {
        [SerializeField] private float canvasAppearDuration;
        [SerializeField] private float canvasDisappearDuration;
        [SerializeField] private List<DeckBuildingSet> buildingSets;

        public override void InstallBindings()
        {
            Container.BindInstance(this);
            Container.BindInstance(buildingSets);
        }

        public float GetCanvasAppearDuration() => canvasAppearDuration;
        public float GetCanvasDisappearDuration() => canvasDisappearDuration;
    }
}