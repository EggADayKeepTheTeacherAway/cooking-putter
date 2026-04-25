using UnityEngine;

public class Customer_WaitForFoodState : CustomerState
{
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

    private void EattingFood(Food food)
    {

    }

    private void OrderedFood()
    {
        Food[] foodList = RestaurantManager.Instance.GetAvailableFoodList();

        int randomFoodIndex = Random.Range(0, foodList.Length);

        Food selectedFood = foodList[randomFoodIndex];

        customer.OnOrderedFood?.Invoke(selectedFood);
    }

}
