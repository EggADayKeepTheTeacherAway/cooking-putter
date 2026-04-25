using UnityEngine;

public class Customer_EattingState : CustomerState
{
    private int minEattingTime = 15;
    private int maxEattingTime = 20;

    public Customer_EattingState(Customer customer, StateMachine stateMachine, string animParam) : base(customer, stateMachine, animParam)
    {
    }


    public override void Enter()
    {
        base.Enter();

        int randomEattingTime = Random.Range(minEattingTime, maxEattingTime);

        stateTimer = randomEattingTime;
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer <= 0)
        {
            stateMachine.ChangeState(customer.exitState);
        }
    }
}
