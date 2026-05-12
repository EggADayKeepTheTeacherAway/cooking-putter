using UnityEngine;
using UnityEngine.UI;

// Displays a single queued order
public class OrderItemUI : MonoBehaviour
{
    [SerializeField] private Text customerNameText;
    [SerializeField] private Text foodNameText;

    public void Set(Customer customer, Food food)
    {
        if (customerNameText != null) customerNameText.text = customer.name;
        if (foodNameText != null) foodNameText.text = food.name;
    }
}
