using Base;
using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Utility.MonoBehaviours;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "DeckConditionLineOfSight", story: "If there is a [target] in line of sight in [distance] [angle] navigate to it [HasTarget]", category: "Conditions", id: "8ac744c7c14177d701ba3b8fe3bc3528")]
public partial class DeckConditionLineOfSightCondition : Condition
{
    [SerializeReference] public BlackboardVariable<DeckAgent> Target;
    [SerializeReference] public BlackboardVariable<float> Distance;
    [SerializeReference] public BlackboardVariable<float> Angle;
    [SerializeReference] public BlackboardVariable<bool> HasTarget;

    public override bool IsTrue()
    {
        var cachedTransform = GameObject.transform;
        var halfAngle = Angle.Value / 2;

        var dividerCount = Angle.Value / 3f + 1;

        var hasEnemyInSight = false;
        var foundEnemies = new HashSet<DeckAgent>();
        for (var i = 0; i < dividerCount; i++)
        {
            var angle = i * 6 - halfAngle;
            Debug.DrawLine(cachedTransform.position + Vector3.up / 2f, Quaternion.Euler(0, angle, 0) * cachedTransform.forward * Distance + cachedTransform.position + Vector3.up / 2f, Color.green);
            if (Physics.Raycast(cachedTransform.position + Vector3.up / 2f, Quaternion.Euler(0, angle, 0) * cachedTransform.forward, out var hit, Distance, 1 << 6))
            {
                if (hit.transform.TryGetComponentInParent<DeckAgent>(out var agent))
                {
                    foundEnemies.Add(agent);
                    hasEnemyInSight = true;
                }
            }
        }

        var minDistance = float.MaxValue;
        DeckAgent currentEnemy = null;

        foreach (var enemy in foundEnemies)
        {
            var currentDistance = Vector3.Distance(cachedTransform.position, enemy.transform.position);
            if (currentDistance < minDistance)
            {
                minDistance = currentDistance;
                currentEnemy = enemy;
            }
        }

        Target.Value = currentEnemy;
        HasTarget.Value = hasEnemyInSight;
        return hasEnemyInSight;
    }
}