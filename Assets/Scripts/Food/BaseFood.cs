using System.Collections.Generic;
using UnityEngine;

public class BaseFood : IFood
{
    private Food sourceFood;
    private Sprite sprite;
    private List<ItemData> ingredients;
    private List<Step> steps;
    private string foodName;
    private int currentStep = 0;
    public bool isDone { get; private set; } = false;

    public BaseFood(Food food)
    {
        sourceFood = food;
        foodName = food.FoodName;
        sprite = food.Icon;
        ingredients = food.Ingredients;
        steps = food.Steps;
    }

    public Food SourceFood => sourceFood;

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
        if (steps == null || steps.Count == 0)
        {
            MarkDone();
            return;
        }

        currentStep += 1;
        if (currentStep >= steps.Count)
            isDone = true;

    } 

    public void MarkDone()
    {
        currentStep = steps != null ? steps.Count : 0;
        isDone = true;
    }

    public Step CurrentStep()
    {
        if (steps == null || currentStep >= steps.Count) return null;
        return steps[currentStep];
    }

    public void ResetState()
    {
        currentStep = 0;
        isDone = false;
    }
}
