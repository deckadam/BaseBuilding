using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "PatrolToPoints", story: "Patrol [Agent] between given [points] with delay between [Min] and [Max] ", category: "Action", id: "a36b1479a3359a18a4cd7ce2f1faa17b")]
public partial class PatrolToPointsAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<List<Vector3>> Points;
    [SerializeReference] public BlackboardVariable<float> Min;
    [SerializeReference] public BlackboardVariable<float> Max;

    private NavMeshAgent _navMeshAgent;
    private Animator _animator;
    private int _index = 0;
    private int _speedParameter;
    private const float StoppingDistance = 0.5f;

    private float _currentWaitTime = 0f;
    private float _waitTime;
    private bool _isWaiting;

    protected override Status OnStart()
    {
        if (Agent.Value == null || Points.Value == null || Points.Value.Count == 0)
            return Status.Failure;

        _navMeshAgent = Agent.Value.GetComponent<NavMeshAgent>();
        _animator = Agent.Value.GetComponentInChildren<Animator>();
        _speedParameter = Animator.StringToHash("MovementSpeed");
        SetNextDestination();
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (_navMeshAgent == null) return Status.Failure;

        if (!_isWaiting && !_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= StoppingDistance)
        {
            _index = (_index + 1) % Points.Value.Count;
            _currentWaitTime = 0;
            _waitTime = UnityEngine.Random.Range(Min.Value, Max.Value);
            _isWaiting = true;
        }

        if (_animator != null && _speedParameter != 0)
        {
            var currentSpeed = _navMeshAgent.velocity.magnitude;
            _animator.SetFloat(_speedParameter, currentSpeed);
        }

        _currentWaitTime += Time.deltaTime;

        if (_waitTime <= _currentWaitTime && !_navMeshAgent.pathPending)
        {
            SetNextDestination();
            _isWaiting = false;
            _currentWaitTime = 0;
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
    }

    private void SetNextDestination()
    {
        if (Points.Value != null && Points.Value.Count > 0)
        {
            _navMeshAgent.SetDestination(Points.Value[_index]);
        }
    }
}