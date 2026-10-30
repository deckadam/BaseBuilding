using Base;
using Components.DamageDealer;
using Unity.Behavior;
using UnityEngine;

namespace Utility.Animators
{
    public class DeckAnimatorEventPropagator : MonoBehaviour
    {
        public void OnAttack()
        {
            var agent = GetComponentInParent<DeckAgent>();
            agent.GetComponent<BehaviorGraphAgent>().GetVariable("Target", out BlackboardVariable<DeckAgent> target);
            agent.GetDeckComponent<DeckComponentDamageDealer>().DealDamage(target.Value.GetComponent<DeckAgent>());
        }
    }
}