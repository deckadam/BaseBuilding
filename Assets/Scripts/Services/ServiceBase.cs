using UnityEngine;

namespace Deck.Services
{
    public class ServiceBase : MonoBehaviour
    {
        private bool _hasWarmedUp = false;

        public virtual int GetWarmUpIndex()
        {
            return 0;
        }

        public void ControlledWarmUp(int index)
        {
            if (GetWarmUpIndex() != index || _hasWarmedUp) return;
            
            WarmUp();
            _hasWarmedUp = true;
        }

        public virtual void WarmUp()
        {
        }

        public virtual void Initialize()
        {
        }

        public virtual void DeInitialize()
        {
        }
    }
}