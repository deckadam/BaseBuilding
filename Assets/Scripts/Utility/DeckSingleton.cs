using UnityEngine;

namespace Utility
{
    public class DeckSingleton<T> : MonoBehaviour where T : DeckSingleton<T>
    {
        private static T Instance;

        private bool _isInitialized;

        public static T ins
        {
            get
            {
                if (Instance != null) return Instance;
                Instance = FindObjectOfType(typeof(T)) as T;

                if (Instance == null)
                {
                    DeckLogger.Warning(typeof(T).Name + " not found in hierarchy creating an instance");
                    Instance = new GameObject().AddComponent<T>();
                }

                if (Instance != null && !Instance._isInitialized) ins.Initialize();
                return Instance;
            }
        }

        protected virtual void Initialize()
        {
            _isInitialized = true;
        }
    }
}