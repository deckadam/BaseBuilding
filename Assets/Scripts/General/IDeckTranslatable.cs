using UnityEngine;

namespace General
{
    public interface IDeckTranslatable
    {
        Vector3 GetPosition();
        void SetPosition(Vector3 position);
        void SetPosition(IDeckTranslatable positioner);

        Quaternion GetRotation();
        Vector3 GetRotationEuler();
        void SetRotation(Vector3 rotation);
        void SetRotation(Quaternion rotation);
        void SetRotation(IDeckTranslatable rotater);

        Vector3 GetScale();
        void SetScale(Vector3 scale);
        void SetScale(IDeckTranslatable scaler);
    }
}