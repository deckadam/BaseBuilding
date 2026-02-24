using General;
using UnityEngine;

namespace Services.Raid.Tools
{
    public class DeckRaidStartPoint : MonoBehaviour, IDeckTranslatable
    {
        public Vector3 GetPosition()
        {
            return transform.position;
        }

        public void SetPosition(Vector3 position)
        {
        }

        public void SetPosition(IDeckTranslatable positioner)
        {
            
        }

        public Quaternion GetRotation()
        {
            return Quaternion.identity;
        }

        public Vector3 GetRotationEuler()
        {
            return Vector3.zero;
        }

        public void SetRotation(Vector3 rotation)
        {
        }

        public void SetRotation(Quaternion rotation)
        {
        }

        public void SetRotation(IDeckTranslatable rotater)
        {
        }

        public Vector3 GetScale()
        {
            return Vector3.one;
        }

        public void SetScale(Vector3 scale)
        {
        }

        public void SetScale(IDeckTranslatable scaler)
        {
        }
    }
}