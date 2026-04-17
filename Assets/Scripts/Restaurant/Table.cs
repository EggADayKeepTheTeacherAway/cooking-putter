using UnityEngine;

public class Table : MonoBehaviour
{
    private Seat[] seats;

    private void Awake()
    {
        seats = GetComponentsInChildren<Seat>();

        RestaurantManager.Instance.RegisterTable(this);
    }
}
