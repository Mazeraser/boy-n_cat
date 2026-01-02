using UnityEngine;

namespace Codebase.AI
{
    public class ArrestState : AIStateBase
    {
        private Transform _arrestZone;
        private bool _isLeadingToZone;
        private bool _conversationStarted;
        
        public ArrestState(AIStateMachine machine) : base(machine) { }
        
        public override void Enter()
        {
            Debug.Log($"{stateMachine.name} арестовал игрока!");
            _isLeadingToZone = false;
            _conversationStarted = false;
            
            // TODO: Заблокировать управление игроком
            // TODO: Воспроизвести анимацию захвата
            
            stateMachine.OnPlayerArrested?.Invoke();
            
            FindArrestZone();
        }
        
        public override void Update()
        {
            if (!_isLeadingToZone)
            {
                StartLeadingToZone();
                return;
            }
            
            if (!_conversationStarted && ReachedArrestZone())
            {
                StartConversation();
            }
        }
        
        private void FindArrestZone()
        {
            _arrestZone = stateMachine.ArrestZone;
        }
        
        private void StartLeadingToZone()
        {
            if (_arrestZone == null)
            {
                // Если зоны нет - сразу начать "разговор"
                StartConversation();
                return;
            }
            
            // TODO: Реализовать движение к зоне ареста вместе с игроком
            _isLeadingToZone = true;
        }
        
        private bool ReachedArrestZone()
        {
            return Vector3.Distance(stateMachine.transform.position, _arrestZone.position) < 1f;
        }
        
        private void StartConversation()
        {
            _conversationStarted = true;
            Debug.Log("Начался 'разговор' с игроком");
            
            // TODO: Запустить диалоговую систему
            // TODO: Предложить выбор: заплатить или вызвать полицию
        }
        
        private void FinishConversation()
        {
            Debug.Log("Разговор завершен, возврат к патрулированию");
            
            // TODO: Разблокировать управление игроком
            // TODO: Применить последствия (деньги/арест)
            
            stateMachine.ChangeState(AIState.Patrol);
        }
        
        private void FinishConversationWrapper()
        {
            FinishConversation();
        }
    }
}