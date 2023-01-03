using UnityEngine;
using Zenject;

namespace Deck.Data.Gizmo
{
    [CreateAssetMenu(fileName = "Deck Gizmo Data", menuName = "Deck/Installer/Deck Gizmo Data", order = 0)]
    public class DeckGizmoData : ScriptableObjectInstaller
    {
        public bool drawGizmos;

        public override void InstallBindings()
        {
            Container.BindInstance(this);
        }
    }
}