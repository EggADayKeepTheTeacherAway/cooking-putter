using UnityEngine;

public class Customer_WaitForFoodState : CustomerState
{
    private Food selectedFood;

    public Customer_WaitForFoodState(Customer customer, StateMachine stateMachine, string animParam) : base(customer, stateMachine, animParam)
    {
    }

    public override void Enter()
    {
        base.Enter();

        customer.OnRecievedFood += EattingFood;

        OrderedFood();

    }


    public override void Exit()
    {
        base.Exit();

        customer.OnRecievedFood -= EattingFood;
    }

    private void EattingFood(Food recievedFood)
    {
        if (recievedFood != selectedFood) return;

        stateMachine.ChangeState(customer.eattingState);
    }

    private void OrderedFood()
    {
        Food[] foodList = RestaurantManager.Instance.GetAvailableFoodList();

        int randomFoodIndex = Random.Range(0, foodList.Length);

        selectedFood = foodList[randomFoodIndex];

        customer.OnOrderedFood?.Invoke(selectedFood);
    }

}
