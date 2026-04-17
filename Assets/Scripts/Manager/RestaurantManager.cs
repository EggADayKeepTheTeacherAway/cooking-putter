using System.Collections.Generic;
using UnityEngine;

public class RestaurantManager : MonoBehaviour
{
    private static RestaurantManager instance;

    public static RestaurantManager Instance => instance;

    private List<Table> tables;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void RegisterTable(Table table) => tables.Add(table);

}
