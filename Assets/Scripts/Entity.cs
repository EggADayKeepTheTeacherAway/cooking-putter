using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Movement Detail")]
    [SerializeField] protected float moveSpeed;

    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected Animator anim;

    public StateMachine stateMachine { get; private set; }
    
    public Entity_IdleState idleState { get; private set; }
    public Entity_MoveState moveState { get; private set; }

    public Vector2 moveInput { get; private set; }

    private PlayerInputSet input;


    private void Awake()
    {

        stateMachine = new StateMachine();

        idleState = new Entity_IdleState (this, stateMachine, "idle");
        moveState = new Entity_MoveState(this, stateMachine, "move");


        stateMachine.Initialize(idleState);        
    }

    private void OnEnable()
    {
        input.Enable();

        input.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Move.canceled += context => moveInput = Vector2.zero;

    }

    private void OnDisable()
    {
        input.Disable();
    }
}
