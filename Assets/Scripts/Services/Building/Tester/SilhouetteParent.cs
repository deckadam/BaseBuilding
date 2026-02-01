using UnityEngine;

namespace Services.Building.Tester
{
    public class SilhouetteParent : MonoBehaviour
    {
        private void OnDisable()
        {
            Debug.LogError("Disable");
        }
    }
}