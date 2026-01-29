using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Codebase.Mechanics.PathfinderSystem;

namespace Codebase.Mechanics.AI
{
    public class PatrolState : AIStateBase
    {
        private List<GraphNode> _availablePoints = new List<GraphNode>();
        private Transform _currentTarget;
        private int _patrolCount;
        private int _maxPatrols;

        public PatrolState(AIStateMachine machine, int maxPatrols=3) : base(machine)
        {
            _maxPatrols = maxPatrols;
        }
        
        public override void Enter()
        {
            //TODO: Воспроизвести анимацию патруля
            Debug.Log($"{stateMachine.name} начал патрулирование");
            _patrolCount = 0;
            _availablePoints = new List<GraphNode>();
            _availablePoints.Add(stateMachine.CurrentNode);
            SelectNextPatrolPoint();
        }
        
        public override void Update()
        {
            if (_currentTarget == null)
            {
                stateMachine.ChangeState(AIState.Idle);
                return;
            }
            MoveToTarget();
            
            if (Vector3.Distance(stateMachine.transform.position, _currentTarget.position) < 0.5f)
            {
                _patrolCount++;
                if (_patrolCount >= _maxPatrols)
                    stateMachine.ChangeState(AIState.Idle);
                else
                    SelectNextPatrolPoint();
            }
            
            if (IsPlayerStealing())
                stateMachine.NotifyPlayerStoleItem();
        }

        private GraphNode GetNearestAccessiblePoint(List<GraphNode> availablePoints)
        {
            if (availablePoints == null || availablePoints.Count == 0)
                return null;

            GraphNode nearestPoint = null;
            float minDistance = float.MaxValue;

            foreach (var point in availablePoints)
            {
                if (point == null) continue;

                if (!HasObstacleBetween(stateMachine.transform.position, point.transform.position))
                {
                    float distance = Vector3.Distance(stateMachine.transform.position, point.transform.position);
                    if (distance < minDistance || (distance==minDistance&&Random.Range(0,2)==0) )
                    {
                        minDistance = distance;
                        nearestPoint = point;
                    }
                }
            }

            return nearestPoint ?? availablePoints[0];
        }

        private bool HasObstacleBetween(Vector3 from, Vector3 to)
        {
            Vector3 direction = to - from;
            float distance = direction.magnitude;
            
            RaycastHit2D hit = Physics2D.Raycast(from, direction.normalized, distance, 
                LayerMask.GetMask("Obstacle"));

            if (hit.collider != null)
            {
                Debug.DrawLine(from, hit.point, Color.red);
                return true;
            }
            
            Debug.DrawLine(from, to, Color.green);
            return false;
        }
        private void SelectNextPatrolPoint()
        {
            if (_availablePoints.Count == 0) return;
            
            int randomGraphIndex=Random.Range(0, _availablePoints.Count);
            
            GraphNode target = GetNearestAccessiblePoint(_availablePoints);
            _currentTarget = target.transform;
            _availablePoints = target.ReachableNodes.ToList();
            stateMachine.CurrentNode = _availablePoints[randomGraphIndex];
            
            Debug.Log($"{stateMachine.name} выбрал новую точку патруля: {_currentTarget.name}");
        }
        
        private void MoveToTarget()
        {
            Vector2 direction= _currentTarget.position - stateMachine.transform.position;
            _movableSystem?.Turn(direction.normalized);
        }
    }
}