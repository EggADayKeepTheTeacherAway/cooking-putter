using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Movement Detail")]
    public float moveSpeed;
    public float runSpeedModifier = 1.5f;

    public Rigidbody2D rb;
    public Animator anim;

    public StateMachine stateMachine { get; private set; }
    
    public Entity_IdleState idleState { get; private set; }
    public Entity_MoveState moveState { get; private set; }

    private Vector2 currentVelocity;
    public Vector2 moveInput { get; private set; }

    public PlayerInputSet input { get; private set; }


    private void Awake()
    {
        input = new PlayerInputSet();


        stateMachine = new StateMachine();

        idleState = new Entity_IdleState (this, stateMachine, "idle");
        moveState = new Entity_MoveState(this, stateMachine, "move");


        stateMachine.Initialize(idleState);        
    }

    private void Update()
    {
        stateMachine.CallUpdateCurrentState();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = currentVelocity;
    }

    private void OnEnable()
    {
        input.Enable();

        input.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Move.canceled += ctx => moveInput = Vector2.zero;

    }

    private void OnDisable()
    {
        input.Disable();
    }

    public void SetVelocity(float velocityX, float velocityY)
    {
        currentVelocity = new Vector2(velocityX, velocityY);
    }
}
