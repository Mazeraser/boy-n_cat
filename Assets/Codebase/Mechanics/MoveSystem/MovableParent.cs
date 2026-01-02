using UnityEngine;

namespace Codebase.Mechanics.MoveSystem
{
    public abstract class MovableParent : MonoBehaviour
    {
        public delegate void MovePerformed();
        public MovePerformed MoveDelegate;
        
        private Rigidbody2D rb;

        public virtual void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        protected void Move(Vector2 direction, float speed)
        {
            Vector2 movement = direction * speed;
            rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);
            MoveDelegate?.Invoke();
        }
    }
}