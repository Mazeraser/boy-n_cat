using UnityEngine;
using Codebase.Mechanics.PathfinderSystem;
using System.Collections.Generic;

namespace Codebase.AI
{
    public class ChaseState : AIStateBase
    {
        private float _chaseTimer;
        private float _maxChaseTime;
        private float _pathUpdateInterval = 3f;
        private float _pathUpdateTimer;
        private List<GraphNode> _currentPath;
        private int _currentPathIndex;

        public ChaseState(AIStateMachine machine, float maxChaseTime=10f) : base(machine)
        {
            _maxChaseTime = maxChaseTime;
        }
        
        public override void Enter()
        {
            // TODO: Воспроизвести звук/анимацию обнаружения
            // TODO: Активировать визуальные эффекты (восклицательный знак и т.д.)
            Debug.Log($"{stateMachine.name} начал погоню!");
            
            UpdatePathToPlayer();
            
            _chaseTimer = 0f;
            _pathUpdateTimer = 0f;
            _currentPathIndex = 0;
        }
        public override void Exit()
        {
            _currentPath = null;
            _currentPathIndex = 0;
        }
        
        public override void Update()
        {
            _chaseTimer += Time.deltaTime;
            _pathUpdateTimer += Time.deltaTime;
            
            if (_chaseTimer >= _maxChaseTime)
            {
                stateMachine.ChangeState(AIState.Patrol);
                return;
            }
            if (_pathUpdateTimer >= _pathUpdateInterval)
            {
                UpdatePathToPlayer();
                _pathUpdateTimer = 0f;
            }
            
            ChasePlayer();
            
            if (CanArrestPlayer())
                stateMachine.ChangeState(AIState.Arrest);
            if (LostPlayer())
                stateMachine.ChangeState(AIState.Patrol);
        }
        
        private void UpdatePathToPlayer()
        {
            if (stateMachine.CurrentNode != null && stateMachine.Player != null)
            {
                _currentPath = SearchOnWidth.Search(stateMachine.Player, stateMachine.CurrentNode);
                _currentPathIndex = 0;
                
                if (_currentPath != null && _currentPath.Count > 0)
                {
                    Debug.Log($"Path updated with {_currentPath.Count} nodes");
                    foreach (var node in _currentPath) 
                    {
                        Debug.Log(node.name);
                    }
                }
                else
                {
                    Debug.LogWarning("Failed to find path to player");
                }
            }
        }
        private bool HasDirectPathToTarget(Vector3 targetPosition)
        {
            Vector3 direction = (targetPosition - stateMachine.transform.position).normalized;
            float distance = Vector3.Distance(stateMachine.transform.position, targetPosition);
            
            RaycastHit2D hit = Physics2D.Raycast(stateMachine.transform.position, direction, distance, 
                LayerMask.GetMask("Obstacle"));

            return hit.collider == null || hit.collider.CompareTag("Player");
        }
        private void ChasePlayer()
        {
            if (_currentPath == null || _currentPath.Count == 0)
            {
                UpdatePathToPlayer();
                return;
            }
            
            if (_currentPathIndex < _currentPath.Count && 
                Vector3.Distance(stateMachine.transform.position, _currentPath[_currentPathIndex].transform.position) < 1f)
            {
                _currentPathIndex++;
                stateMachine.CurrentNode = _currentPath[_currentPathIndex - 1];
            }

            bool canGoDirectly = HasDirectPathToTarget(stateMachine.Player.position) && 
                                HasLineOfSightToPlayer();

            if (_currentPathIndex < _currentPath.Count && !canGoDirectly)
            {
                MoveToNode(_currentPath[_currentPathIndex]);
            }
            else
            {
                _chaseTimer = 0;
                MoveToPlayerDirectly();
            }
        }
        private bool HasObstacleInDirection(Vector3 direction, float checkDistance = 2f, float angle=15f)
        {
            Vector3 from = stateMachine.transform.position;
            for(int i=-3;i<=3;i++)
            {
                RaycastHit2D hit = Physics2D.Raycast(from, Quaternion.Euler(0,0,angle*i) * direction, checkDistance, 
                    LayerMask.GetMask("Obstacle"));

                if (hit.collider != null)
                {
                    Debug.DrawLine(from, hit.point, Color.blue);
                    return true;
                }
            }
            
            Debug.DrawRay(from, direction * checkDistance, Color.green);
            return false;
        }
        private Vector3 FindAlternativeDirection(Vector3 originalDirection, Vector3 targetPosition, float angleStep = 45f)
        {
            Vector3 bestDirection = originalDirection;
            float bestScore = float.MaxValue;

            for (int i = 1; i <= 4; i++)
            {
                float[] angles = { i * angleStep, -i * angleStep };
                
                foreach (float angle in angles)
                {
                    Vector3 testDirection = Quaternion.Euler(0, 0, angle) * originalDirection;
                    
                    if (!HasObstacleInDirection(testDirection))
                    {
                        Vector3 directionToTarget = (targetPosition - stateMachine.transform.position).normalized;
                        float angleDifference = Vector3.Angle(testDirection, directionToTarget);
                        float distanceToTarget = Vector3.Distance(stateMachine.transform.position, targetPosition);
                        
                        float score = angleDifference * 0.7f + distanceToTarget * 0.3f;
                        
                        if (score < bestScore)
                        {
                            bestScore = score;
                            bestDirection = testDirection;
                        }
                    }
                }
                
                if (bestScore < float.MaxValue)
                    break;
            }

            return bestDirection;
        }
        private void MoveToNode(GraphNode targetNode)
        {
            if (_movableSystem != null && targetNode != null)
            {
                Vector3 directionToTarget = (targetNode.transform.position - stateMachine.transform.position).normalized;
                
                if (HasObstacleInDirection(directionToTarget))
                {
                    Vector3 alternativeDirection = FindAlternativeDirection(directionToTarget, targetNode.transform.position);
                    _movableSystem?.Turn(alternativeDirection);
                }
                else
                {
                    _movableSystem?.Turn(directionToTarget);
                }
            }
        }
        private void MoveToPlayerDirectly()
        {
            if (_movableSystem != null && stateMachine.Player != null)
            {
                Vector3 direction = (stateMachine.Player.position - stateMachine.transform.position).normalized;
                _movableSystem.Turn(direction);
            }
        }
        
        private bool CanArrestPlayer()
        {
            return IsPlayerInRange(stateMachine.ArrestRange) && HasLineOfSightToPlayer();
        }
        private bool LostPlayer()
        {
            return _chaseTimer>_maxChaseTime;
        }
    }
}