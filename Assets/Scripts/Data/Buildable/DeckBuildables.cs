using UnityEngine;
using Zenject;

namespace Deck.Data.Buildable
{
    [CreateAssetMenu(fileName = "Deck Buildable", menuName = "Deck/Binder/Buildables", order = 0)]
    public class DeckBuildables : ScriptableObjectInstaller
    {
        [SerializeField] private DeckBuildable[] buildables;

        public override void InstallBindings()
        {
            Container.BindInstance(buildables);
        }
    }
}