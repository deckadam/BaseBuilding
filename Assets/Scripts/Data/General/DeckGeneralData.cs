using Deck.Player;
using UnityEngine;
using Zenject;

namespace Deck.Data.General
{
    [CreateAssetMenu(menuName = "Deck/Installer/General", fileName = "Deck General Data")]
    public class DeckGeneralData : ScriptableObjectInstaller
    {
        public DeckCoreAgent coreAgentPrefab;

        public override void InstallBindings()
        {
            Container.BindInstance(this);
        }
    }
}