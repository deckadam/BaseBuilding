using UnityEngine;
using Zenject;

namespace Deck.Services
{
    public class DeckServiceBase : MonoBehaviour
    {
        private bool _hasWarmedUp;

        public void ControlledWarmUp(int index)
        {
            if (GetWarmUpIndex() != index || _hasWarmedUp) return;

            WarmUp();
            _hasWarmedUp = true;
        }

        private void OnDestroy()
        {
            DeInitialize();
        }

        public virtual int GetWarmUpIndex()
        {
            return 0;
        }

        public virtual void WarmUp()
        {
        }

        public virtual void DeInitialize()
        {
        }

        public virtual void Initialize()
        {
        }
    }
}