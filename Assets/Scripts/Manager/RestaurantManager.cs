using System.Collections.Generic;
using UnityEngine;

public class RestaurantManager : MonoBehaviour
{
    private static RestaurantManager instance;

    public static RestaurantManager Instance => instance;

    private List<Table> tables;

    private List<Customer[]> customerGroups;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        tables = new List<Table>();
    }

    public void RegisterTable(Table table) => tables.Add(table);

    public List<Table> GetAvailableTables() => tables.FindAll(table => !table.isTaken);

}
