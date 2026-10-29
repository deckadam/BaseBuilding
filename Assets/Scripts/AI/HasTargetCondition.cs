using System;
using Services;
using Services.Finder;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

[Serializable, GeneratePropertyBag]
[Condition(name: "HasTarget", story: "If there is a [target] navigate to it", category: "Conditions", id: "a65d0bea646f2d8baaff28a70f641dde")]
public partial class HasTargetCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    public override bool IsTrue()
    {
        var raiders = DeckServiceProvider.GetService<DeckServiceFinder>().GetAgentsWithTag(DeckActionTag.Raider);
        if (raiders.Count == 0)
        {
            return false;
        }

        GameObject target = null;
        var hasFoundTarget = false;
        var minDistance = 1000f;
        foreach (var raider in raiders)
        {
            if (!raider.IsAgentReady)
            {
                continue;
            }

            var tempDistance = Vector3.Distance(raider.transform.position, GameObject.transform.position);
            if (tempDistance < minDistance)
            {
                minDistance = tempDistance;
                target = raider.gameObject;
                hasFoundTarget = true;
            }
        }

        Target.Value = target;
        return hasFoundTarget;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}