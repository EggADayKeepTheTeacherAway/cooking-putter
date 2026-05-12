using System.Collections.Generic;
using UnityEngine;

public class BaseFood : IFood
{
    private Sprite sprite;
    private List<ItemData> ingredients;
    private List<Step> steps;
    private string foodName;
    private int currentStep = 0;
    public bool isDone { get; private set; } = false;

    public BaseFood(Food food)
    {
        foodName = food.FoodName;
        sprite = food.Icon;
        ingredients = food.Ingredients;
        steps = food.Steps;
    }

    public List<ItemData> GetIngredients()
    {
        return ingredients;
    }

    public bool IsDone()
    {
        return isDone;
    }

    public string GetFoodName()
    {
        return foodName;
    }

    public Sprite GetIcon()
    {
        return sprite;
    }

    public void NextStep()
    {
        if (isDone) return;

        currentStep += 1;
        if (currentStep >= steps.Count)
            isDone = true;

    } 

    public Step CurrentStep()
    {
        if (currentStep >= steps.Count) return null;
        return steps[currentStep];
    }

    public void ResetState()
    {
        currentStep = 0;
        isDone = false;
    }
}
