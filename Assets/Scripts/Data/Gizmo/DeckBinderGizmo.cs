using UnityEngine;
using Zenject;

namespace Deck.Data.Gizmo
{
    [CreateAssetMenu(fileName = "Deck Data Gizmo", menuName = "Deck/Binder/Gizmo", order = 0)]
    public class DeckBinderGizmo : ScriptableObjectInstaller
    {
        public bool drawGizmos;

        public override void InstallBindings()
        {
            Container.BindInstance(this);
        }
    }
}