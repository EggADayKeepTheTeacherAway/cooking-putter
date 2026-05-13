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
        if (RestaurantManager.Instance == null)
        {
            Debug.LogError("RestaurantManager.Instance not found.");
            return;
        }

        Food[] foodList = RestaurantManager.Instance.GetAvailableFoodList();

        if (foodList == null || foodList.Length == 0)
        {
            Debug.LogError("No food is assigned in RestaurantManager. Please populate the foodList or check Assets/Data/Food folder.");
            return;
        }

        // Select a random food from the available list
        int randomFoodIndex = Random.Range(0, foodList.Length);
        selectedFood = foodList[randomFoodIndex];

        if (selectedFood == null)
        {
            Debug.LogError("Selected food is null. Check RestaurantManager.foodList.");
            return;
        }

        // Notify that a food was ordered
        customer.OnOrderedFood?.Invoke(selectedFood);

        // Queue the order in the Food Service Manager
        FoodServiceManager.GetOrCreateInstance().OrderFood(customer, selectedFood);
    }

}
