using UnityEngine;

public class Customer : Entity
{
    [SerializeField] private SpriteRenderer sr;
    private CustomerGroup group;
    private Vector2 moveTarget;

    public Entity_IdleState idleState;
    public Customer_LookingForSeatState findSeatState;
    


    protected override void Awake()
    {
        base.Awake();

        idleState = new Entity_IdleState(this, stateMachine, "idle");
        findSeatState = new Customer_LookingForSeatState(this, stateMachine, "walk");

    }

    private void Start()
    {
        stateMachine.Initialize(findSeatState);
    }

    private void FixedUpdate()
    {
        Vector2 current = rb.position;

        Vector2 newPos = Vector2.MoveTowards(current, moveTarget, moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);
    }

    public void SetGroup(CustomerGroup group)
    {
        this.group = group;
    }

    public Table GetTable()
    {
        return group.table;
    }

    public void SetMoveTarget(Vector2 newTarget) => moveTarget = newTarget;
    public Vector2 GetMoveTarget() => moveTarget;


    public void ResetSortingOrder() => sr.sortingOrder = 0;
  
    public void TopSortingOrder() => sr.sortingOrder = 1;


    public void BottomSortingOrder() => sr.sortingOrder = -1;
}
