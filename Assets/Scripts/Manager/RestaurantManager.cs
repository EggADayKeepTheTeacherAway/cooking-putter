using System.Collections.Generic;
using UnityEngine;

public class RestaurantManager : MonoBehaviour
{
    [SerializeField] private CustomerSpawner spawner;
    [SerializeField] private RandomTimer spawnDelay;

    private float spawnTimer;

    private static RestaurantManager instance;

    public static RestaurantManager Instance => instance;

    private List<Table> tables;

    private List<List<Customer>> customerGroups;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        tables = new List<Table>();
        customerGroups = new List<List<Customer>>();
    }

    private void Update()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0)
        {
            spawnTimer = spawnDelay.GetRandomDelay();
            customerGroups.Add(spawner.SpawnCustomer());
        }
    }

    public void RegisterTable(Table table) => tables.Add(table);

    public List<Table> GetAvailableTables() => tables.FindAll(table => !table.isTaken);

}
