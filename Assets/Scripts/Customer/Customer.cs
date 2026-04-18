using UnityEngine;

public class Customer : Entity
{
    public Entity_IdleState idleState;
    public Customer_LookingForSeatState findSeatState;
    public SpriteRenderer sr;
 
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
}
