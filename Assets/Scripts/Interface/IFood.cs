using System.Collections.Generic;
using UnityEngine;

public interface IFood
{
    List<ItemData> GetIngredients();

    bool IsDone();

    string GetFoodName();

    Sprite GetIcon();

    void NextStep();

    Step CurrentStep();
    
    void ResetState();
}
