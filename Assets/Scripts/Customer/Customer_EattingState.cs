using UnityEngine;

public class Customer_EattingState : CustomerState
{
    private bool finishedEating = false;

    private int minEattingTime = 15;
    private int maxEattingTime = 20;

    public Customer_EattingState(Customer customer, StateMachine stateMachine, string animParam) : base(customer, stateMachine, animParam)
    {
    }


    public override void Enter()
    {
        base.Enter();

        finishedEating = false;

        customer.GetGroup().OnGroupFinishedEatting += OnGroupFinishedEatting;

        int randomEattingTime = Random.Range(minEattingTime, maxEattingTime);

        stateTimer = randomEattingTime;
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer <= 0 && !finishedEating)
        {
            finishedEating = true;
            
            // Change food to dirty dish
            FoodServiceManager.GetOrCreateInstance().ChangeFoodToDirtyDish(customer);
            
            customer.GetGroup().FinishedEatting();
        }
    }

    public override void Exit()
    {
        base.Exit();

        customer.GetGroup().OnGroupFinishedEatting -= OnGroupFinishedEatting;
    }

    private void OnGroupFinishedEatting()
    {
        if (customer != customer.GetGroup().customers[0])
        {
            stateMachine.ChangeState(customer.exitState);
            return;
        }

        int customerCount = customer.GetGroup().customers.Count;

        int totalPayment = 0;

        for (int i = 0; i < customerCount; i++)
        {
            totalPayment += Random.Range(50, 101);
        }

        PlayerDataManager.Instance.AddMoney(totalPayment);

        stateMachine.ChangeState(customer.exitState);
    }
}
