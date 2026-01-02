using Codebase.Mechanics.MoveSystem;
using UnityEngine;

namespace Codebase.Infrastructure
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    
    [RequireComponent(typeof(IMovable))]
    [RequireComponent(typeof(CharacterAnimationCommand))]
    
    public class Player : MonoBehaviour
    {
        //TODO: Реализовать управление для n игроков(локально)
        
        private Main inputAction; //TODO: Перенести в контейнер
        private IMovable moveController;
        private CharacterAnimationCommand animCommand;
        private Rigidbody2D rb;
        
        private void Awake()
        {
            inputAction = new Main();
            rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            moveController = GetComponent<IMovable>(); // Находим один раз
            animCommand = GetComponent<CharacterAnimationCommand>(); // Находим один раз
        }
        private void FixedUpdate()
        {
            Vector2 velocity = inputAction.Move.Move.ReadValue<Vector2>();
            moveController.Turn(velocity);
        }
        private void OnEnable()
        {
            inputAction.Enable();
        }
        private void OnDisable()
        {
            inputAction.Disable();
        }
    }
}