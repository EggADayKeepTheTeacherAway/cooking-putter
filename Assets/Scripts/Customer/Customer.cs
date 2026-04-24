using System.Collections.Generic;
using UnityEngine;

public class Customer : Entity
{
    private CustomerGroup group;
    private Queue<Vector2> moveQueue = new Queue<Vector2>();
    private Vector2 currentTarget;
    private bool hasTarget;
    public System.Action OnReachedTarget;

    public Entity_IdleState idleState;
    public Customer_LookingForSeatState findSeatState;
    public Customer_WaitForFoodState waitFoodState;
    


    protected override void Awake()
    {
        base.Awake();

        idleState = new Entity_IdleState(this, stateMachine, "idle");
        findSeatState = new Customer_LookingForSeatState(this, stateMachine, "move");
        waitFoodState = new Customer_WaitForFoodState(this, stateMachine, "idle");

    }

    private void Start()
    {
        stateMachine.Initialize(findSeatState);
    }

    private void FixedUpdate()
    {
        if (!hasTarget) return;

        Vector2 newPos = Vector2.MoveTowards(
            rb.position,
            currentTarget,
            moveSpeed * Time.fixedDeltaTime
        );

        rb.MovePosition(newPos);

        if (Vector2.Distance(rb.position, currentTarget) < 0.05f)
        {
            SetNextTarget();
        }
    }

    public void SetGroup(CustomerGroup group)
    {
        this.group = group;
    }

    public CustomerGroup GetGroup() => group;

    public Table GetTable() => group.table;

    public void SetPath(List<Vector2> points)
    {
        moveQueue.Clear();

        foreach (var p in points)
            moveQueue.Enqueue(p);

        SetNextTarget();
    }

    private void SetNextTarget()
    {
        if (moveQueue.Count == 0)
        {
            hasTarget = false;
            OnReachedTarget?.Invoke();
            return;
        }

        currentTarget = moveQueue.Dequeue();
        hasTarget = true;
    }


    public void ResetSortingOrder() => sr.sortingOrder = 5;
  
    public void SetTopSortingOrder() => sr.sortingOrder = 1;


    public void SetBottomSortingOrder() => sr.sortingOrder = -1;
}
