using UnityEngine;
using Codebase.Mechanics.MoveSystem;

namespace Codebase.AI
{
    public abstract class AIStateBase
    {
        protected AIStateMachine stateMachine;
        protected IMovable _movableSystem;
        
        public AIStateBase(AIStateMachine machine)
        {
            stateMachine = machine;
            _movableSystem = stateMachine.GetMoveSystem ?? null;
            if (_movableSystem == null)
                Debug.LogError($"{stateMachine.name} don't have move system");
        }
        
        public virtual void Enter() { }
        public virtual void Update() { }
        public virtual void Exit() { }
        
        protected bool IsPlayerInRange(float range)
        {
            return Vector3.Distance(stateMachine.transform.position, stateMachine.Player.position) <= range;
        }
        
        bool GetRaycast(Vector2 dir, float distance = 15f)
        {
            bool result = false;
            Vector3 pos = stateMachine.transform.position;

            RaycastHit2D hit = Physics2D.Raycast(pos, dir, distance, 
                LayerMask.GetMask("Player", "Obstacle"));

            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Player"))
                {
                    result = true;
                    Debug.DrawLine(pos, hit.point, Color.green);
                }
                else
                {
                    Debug.DrawLine(pos, hit.point, Color.blue);
                }
            }
            else
            {
                Debug.DrawRay(pos, dir.normalized * distance, Color.red);
            }

            return result;
        }

        protected bool HasLineOfSightToPlayer(int rays = 8, float angle = 60f)
        {
            bool result = false;
            bool a = false;
            bool b = false;
            float j = 0f;

            for (int i = 0; i < rays; i++)
            {
                float x = Mathf.Sin(j);
                float y = Mathf.Cos(j);
                j += angle * Mathf.Deg2Rad / rays;

                Vector2 dir = Quaternion.Euler(0, 0, stateMachine.transform.eulerAngles.z) * new Vector2(x, y);
                if (GetRaycast(dir)) a = true;

                if (x != 0)
                {
                    dir = Quaternion.Euler(0, 0, stateMachine.transform.eulerAngles.z) * new Vector2(-x, y);
                    if (GetRaycast(dir)) b = true;
                }
            }

            if (a || b) result = true;
            return result;
        }
        
        protected bool IsPlayerActuallyStealing()
        {
            // TODO: Проверить через систему инвентаря/предметов, что игрок украл предмет
            return false;
        }
        protected bool IsPlayerStealing()
        {
            // TODO: Реализовать логику обнаружения кражи
            // Проверка расстояния (Done)
            // прямой видимости (Done)
            // и факта кражи 
            return IsPlayerInRange(stateMachine.DetectionRange)
             && HasLineOfSightToPlayer();
            //&& IsPlayerActuallyStealing();
        }
    }
}