using UnityEngine;

namespace Deck.Services.Implementations.ObjectPooling
{
    public interface IDeckPoolable
    {
        void Initialize();
        void DeInitialize();
        MonoBehaviour getMonoBehaviour();
    }
}