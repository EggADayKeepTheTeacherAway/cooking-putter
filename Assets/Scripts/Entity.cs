using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Movement Detail")]
    public float moveSpeed;

    public Rigidbody2D rb;
    public Animator anim;
    public SpriteRenderer sr;

    public StateMachine stateMachine { get; private set; }   


    protected virtual void Awake()
    {
        stateMachine = new StateMachine();
    }

    protected virtual void Update()
    {
        stateMachine.CallUpdateCurrentState();
    } 
}
