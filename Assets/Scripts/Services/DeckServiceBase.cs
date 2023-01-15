using UnityEngine;
using Zenject;

namespace Deck.Services
{
    public class DeckServiceBase : MonoBehaviour
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

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
            DeInitialize();
        }

        protected T GetData<T>() where T : ScriptableObject
        {
            var typeName = typeof(T).Name;
            var result = Resources.Load<T>("Services\\" + typeName);

            if (result == null)
            {
                Debug.LogError(typeName + "   resource not found");
            }
            else
            {
                Debug.Log(typeName + "   resource loaded succesfully");
            }

            return result;
        }

        public virtual void DeInitialize()
        {
        }
    }
}