using UnityEngine;

namespace Services
{
    public class DeckServiceBase : MonoBehaviour
    {
#if UNITY_EDITOR
        public bool showGizmos = true;
#endif
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

        protected virtual void WarmUp()
        {
        }

        protected virtual void DeInitialize()
        {
        }

        public virtual void Initialize()
        {
        }

        public virtual void BeforeGameSessionInitialized()
        {
        }

        public virtual void AfterGameSessionInitialized()
        {
        }

        public virtual void BeforeGameSessionDeinitialized()
        {
        }

        public virtual void BeforeSaveRequest()
        {
        }

        public void OnDrawGizmos()
        {
            if (!showGizmos)
            {
                return;
            }
            
            DrawGizmos();
        }

        protected virtual void DrawGizmos()
        {
            
        }
    }
}