using UnityEngine;

namespace Data.Component
{
    [CreateAssetMenu(fileName = "Deck Data Movement", menuName = "Deck/Data/Component/Movement")]
    public class DeckDataMovement : DeckComponentData
    {
        [SerializeField] private float movementSpeed;
        [SerializeField] private float acceleration;
        [SerializeField] private float angularSpeed;

        public float MovementSpeed => movementSpeed;
        public float Acceleration => acceleration;
        public float AngularSpeed => angularSpeed;
    }
}