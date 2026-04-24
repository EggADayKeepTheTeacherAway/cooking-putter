using UnityEngine;
using static Player;

public class Entity : MonoBehaviour
{
    [Header("Movement Detail")]
    public float moveSpeed;

    public Rigidbody2D rb;
    public Animator anim;
    public SpriteRenderer sr;

    public StateMachine stateMachine { get; private set; }

    public enum FacingDirection { Up, Down, Left, Right }
    public FacingDirection facingDirection { get; private set; } = FacingDirection.Down;


    protected virtual void Awake()
    {
        stateMachine = new StateMachine();
    }

    protected virtual void Update()
    {
        stateMachine.CallUpdateCurrentState();
    }

    protected void UpdateFacingDirection(Vector2 input)
    {
        if (input == Vector2.zero) return;

        if (Mathf.Abs(input.x) >= Mathf.Abs(input.y))
            facingDirection = input.x > 0 ? FacingDirection.Right : FacingDirection.Left;
        else
            facingDirection = input.y > 0 ? FacingDirection.Up : FacingDirection.Down;

        sr.flipX = (facingDirection == FacingDirection.Right);

    }
}
