using System.Collections.Generic;
using UnityEngine;

public class RestaurantManager : MonoBehaviour
{
    // 80% people choose smallest table 20% selfish
    [SerializeField, Range(0f, 1f)] private float selfishRate = 0.2f;
    [SerializeField] private string[] foodList;
    [SerializeField] private CustomerSpawner spawner;
    [SerializeField] private RandomTimer spawnDelay;

    private float spawnTimer;

    private static RestaurantManager instance;

    public static RestaurantManager Instance => instance;

    private List<Table> tables;

    private List<CustomerGroup> customerGroups;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        tables = new List<Table>();
        customerGroups = new List<CustomerGroup>();
    }

    private void Update()
    {
        SpawnCustomer();
    }

    public string[] GetAvailableFoodList() => foodList;

    private bool SpawnCustomer()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0)
        {
            spawnTimer = spawnDelay.GetRandomDelay();

            List<Customer> customers = spawner.SpawnCustomer();

            CustomerGroup group = new CustomerGroup(customers);

            Table t = AssignedTable(group.Count);

            if (t != null)
            {
                group.AssignedTable(t);
                t.Take();
            }

            foreach (var c in customers)
            {
                c.SetGroup(group);
            }

            customerGroups.Add(group);
        }

        return true;
    }

    public void RegisterTable(Table table) => tables.Add(table);

    public List<Table> GetAvailableTables(int amountOfCustomers)
    {
        return tables.FindAll(table => !table.isTaken && table.seatAmount >= amountOfCustomers);
    }

    public Table AssignedTable(int amountOfCustomers)
    {
        List<Table> allAvailableTables = GetAvailableTables(amountOfCustomers);

        if (allAvailableTables.Count == 0)
            return null;

        allAvailableTables.Sort((a, b) => a.seatAmount.CompareTo(b.seatAmount));

        int chosenTableIndex = Random.Range(0, allAvailableTables.Count);

        if (Random.value > selfishRate)
        {
            return allAvailableTables[0];
        }

        return allAvailableTables[Random.Range(0, allAvailableTables.Count)];
    }
}
