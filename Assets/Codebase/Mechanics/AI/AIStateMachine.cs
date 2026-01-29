using UnityEngine;
using System.Collections.Generic;
using Codebase.Mechanics.MoveSystem;
using Codebase.Mechanics.PathfinderSystem;

namespace Codebase.Mechanics.AI
{
    public enum AIState
    {
        Idle,
        Patrol,
        Chase,
        Arrest
    }

    public class AIStateMachine : MonoBehaviour
    {
        [SerializeField] private AIState _currentState = AIState.Idle;
        
        private AIStateBase[] _states;
        private AIStateBase CurrentState => _states[(int)_currentState];
        
        [SerializeField] private Transform _player; //TODO: Реализовать добавление игрока через DI
        [SerializeField] private Transform _arrestZone;
        [SerializeField] private GraphNode _patrolStartPoint;
        [SerializeField] private float _detectionRange = 5f;
        [SerializeField] private float _arrestRange = 2f;

        [SerializeField][Tooltip("Длительность покоя")]
        private float _idleTime=5f;
        [SerializeField][Tooltip("Максимальное количество точек патруля по которому проходит")] 
        private int _maxPatrols=3;
        [SerializeField][Tooltip("Время преследования")] 
        private float _maxChaseTime=6f;

        [HideInInspector]public GraphNode CurrentNode;

        public Transform Player => _player;
        public Transform ArrestZone => _arrestZone;
        public float DetectionRange => _detectionRange;
        public float ArrestRange => _arrestRange;
        
        public System.Action<AIState> OnStateChanged;
        public System.Action OnPlayerDetectedStealing;
        public System.Action OnPlayerArrested;

        private void Awake()
        {
            _states = new AIStateBase[]
            {
                new IdleState(this, _idleTime),
                new PatrolState(this, _maxPatrols),
                new ChaseState(this, _maxChaseTime),
                new ArrestState(this)
            };
        }
        private void Start()
        {
            ChangeState(AIState.Idle);
            CurrentNode = _patrolStartPoint;
        }
        private void Update()
        {
            CurrentState?.Update();
        }

        public void ChangeState(AIState newState)
        {
            CurrentState?.Exit();
            _currentState = newState;
            CurrentState?.Enter();
            OnStateChanged?.Invoke(newState);
        }
        public void NotifyPlayerStoleItem()
        {
            if (_currentState != AIState.Chase && _currentState != AIState.Arrest)
            {
                OnPlayerDetectedStealing?.Invoke();
                ChangeState(AIState.Chase);
            }
        }

        public IMovable GetMoveSystem => GetComponent<IMovable>();

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _detectionRange);
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _arrestRange);
        }
    }
}