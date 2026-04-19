using UnityEngine;

public class Customer : Entity
{
    public Entity_IdleState idleState;
    public Customer_LookingForSeatState findSeatState;
    
    [SerializeField] private SpriteRenderer sr;
    private CustomerGroup group;


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

    public void SetGroup(CustomerGroup group)
    {
        this.group = group;
    }

    public Table GetTable()
    {
        return group.table;
    }

    public void ResetSortingOrder() => sr.sortingOrder = 0;
  
    public void TopSortingOrder() => sr.sortingOrder = 1;


    public void BottomSortingOrder() => sr.sortingOrder = -1;
}
