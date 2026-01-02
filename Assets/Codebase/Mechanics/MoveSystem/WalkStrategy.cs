using UnityEngine;
using Sirenix.OdinInspector;

namespace Codebase.Mechanics.MoveSystem
{
    public class WalkStrategy : MovableParent, IMovable
    {
        [SerializeField, BoxGroup("Settings")]
        [LabelText("Move speed")]
        [Tooltip("Скорость передвижения персонажа")]
        [MinValue(0f)]
        private float _moveSpeed;

        
        [ButtonGroup("SpeedControls")]
        [Button("Increase Speed")]
        private void IncreaseSpeed() => _moveSpeed += 1f;
        [ButtonGroup("SpeedControls")]
        [Button("Decrease Speed")]
        private void DecreaseSpeed() => _moveSpeed = Mathf.Max(0f, _moveSpeed - 1f);

        [ButtonGroup("SpeedControls")]
        [Button("Set default speed")]
        private void SetDefaultSpeed() => _moveSpeed = 5f;
        
        public float Speed
        {
            get { return _moveSpeed; }
        }

        public void Turn(Vector2 direction)
        {
            Move(direction, _moveSpeed);
            if (direction != Vector2.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 90);
            }
            else
                transform.rotation = Quaternion.identity;
        }
    }
}