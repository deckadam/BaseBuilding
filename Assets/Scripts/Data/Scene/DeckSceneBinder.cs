using Unity.Cinemachine;
using UnityEngine;
using Zenject;

namespace Data.Scene
{
    public class DeckSceneBinder : MonoInstaller
    {
        [SerializeField] private CinemachineBrain brain;

        public override void InstallBindings()
        {
            Container.BindInstance(brain).AsSingle();
        }
    }
}