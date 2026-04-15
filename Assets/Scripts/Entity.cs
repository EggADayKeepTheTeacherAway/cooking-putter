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

    private void Awake()
    {
        stateMachine = new StateMachine();

        idleState = new Entity_IdleState (this, stateMachine, "idle");
        moveState = new Entity_MoveState(this, stateMachine, "move");


        stateMachine.Initialize(idleState);        
    }
}
