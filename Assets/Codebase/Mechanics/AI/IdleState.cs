using UnityEngine;

namespace Codebase.AI
{
    public class IdleState : AIStateBase
    {
        private float _idleTimer;
        private float _currentIdleTime;

        public IdleState(AIStateMachine machine, float idleTime=5f) : base(machine)
        {
            _currentIdleTime = idleTime;
        }
        
        public override void Enter()
        {
            // TODO: Воспроизвести анимацию покоя
            Debug.Log($"{stateMachine.name} перешел в состояние Покоя");
            
            _idleTimer = 0f;
        }
        
        public override void Update()
        {
            _idleTimer += Time.deltaTime;
            if (_idleTimer >= _currentIdleTime)
            {
                stateMachine.ChangeState(AIState.Patrol);
            }
            
            if (IsPlayerStealing())
            {
                stateMachine.NotifyPlayerStoleItem();
            }
        }
    }
}