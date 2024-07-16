using Deck.Services;
using Deck.Save;
using Zenject;

namespace Deck.Installers
{
    public class DeckBinderClass : MonoInstaller
    {
        public override void InstallBindings()
        {
            //Class references
            Container.Bind<DeckLoadResolver>().AsSingle();
        }
    }
}