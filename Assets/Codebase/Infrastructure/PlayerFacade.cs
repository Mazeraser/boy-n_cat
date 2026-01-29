using Codebase.Mechanics.MoveSystem;
using Codebase.Mechanics.StealSystem;
using UnityEngine;

namespace Codebase.Infrastructure
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    
    [RequireComponent(typeof(IMovable))]
    [RequireComponent(typeof(CharacterAnimationCommand))]
    [RequireComponent(typeof(RobberComponent))]
    
    public class PlayerFacade : MonoBehaviour
    {
        //TODO: Реализовать управление для n игроков(локально)
        
        private Main inputAction; //TODO: Перенести в контейнер
        private Rigidbody2D rb;

        private IMovable moveController;
        private CharacterAnimationCommand animCommand;
        private RobberComponent robberComponent;

        [SerializeField]private GameObject _stealableItem=null;

        private void StealItem()
        {
            if(_stealableItem!=null && robberComponent.CanSteal(_stealableItem.tag))
                robberComponent.Steal(_stealableItem.GetComponent<ItemMemento>().ItemID);
        }

        private void Awake()
        {
            inputAction = new Main();
        }
        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            moveController = GetComponent<IMovable>(); 
            animCommand = GetComponent<CharacterAnimationCommand>();
            robberComponent = GetComponent<RobberComponent>();
        }
        private void FixedUpdate()
        {
            Vector2 velocity = inputAction.Move.Move.ReadValue<Vector2>();
            moveController.Turn(velocity);
        }
        private void OnEnable()
        {
            inputAction.Enable();
            inputAction.Move.Steal.performed += ctx => StealItem();
        }
        private void OnDisable()
        {
            inputAction.Disable();
        }
        private void OnTriggerStay2D(Collider2D other){
            if(other.CompareTag("Item")){
                _stealableItem = other.gameObject;
                Debug.Log(_stealableItem.name);
            }
        }
        private void OnTriggerExit2D(Collider2D other) => _stealableItem=null;
    }
}