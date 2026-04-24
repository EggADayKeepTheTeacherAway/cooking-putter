using UnityEngine;

public class Customer_WaitForFoodState : CustomerState
{
    public Customer_WaitForFoodState(Customer customer, StateMachine stateMachine, string animParam) : base(customer, stateMachine, animParam)
    {
    }

    public override void Enter()
    {
        base.Enter();

    }
}
