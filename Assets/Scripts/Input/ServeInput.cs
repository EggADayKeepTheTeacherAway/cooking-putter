using UnityEngine;

// Handles player input for serving orders
public class ServeInput : MonoBehaviour
{
    [SerializeField] private KeyCode serveKey = KeyCode.Space;

    private void Update()
    {
        if (Input.GetKeyDown(serveKey))
        {
            if (FoodServiceManager.Instance != null)
                FoodServiceManager.Instance.ServeNextOrder();
        }
    }

    // Hook this to a UI Button's OnClick()
    public void ServeButton()
    {
        if (FoodServiceManager.Instance != null)
            FoodServiceManager.Instance.ServeNextOrder();
    }
}
