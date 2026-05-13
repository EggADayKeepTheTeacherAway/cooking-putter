using System.Collections.Generic;
using UnityEngine;

public class PhasingSeagul : Entity
{
    public static PhasingSeagul Instance { get; private set; }

    public Seat seat { get; private set; }
    private Queue<Vector2> moveQueue = new Queue<Vector2>();
    public Vector2 currentTarget { get; private set; }
    private bool hasTarget;
    public System.Action OnReachedTarget;

    public System.Action<Food> OnOrderedFood;
    public System.Action<Food> OnRecievedFood;

    public Entity_IdleState idleState;
    public Customer_LookingForSeatState findSeatState;
    public Customer_WaitForFoodState waitFoodState;
    public Customer_EattingState eattingState;
    public Customer_ExitState exitState;
    public Customer_NoTableState noTableState;

    protected override void Awake()
    {
        base.Awake();

        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
}
