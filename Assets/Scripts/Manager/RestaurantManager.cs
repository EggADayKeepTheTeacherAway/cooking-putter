using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RestaurantManager : MonoBehaviour
{
    // 80% people choose smallest table 20% selfish
    [SerializeField, Range(0f, 1f)] private float selfishRate = 0.2f;
    [SerializeField] private Food[] foodList;
    [SerializeField] private CustomerSpawner spawner;
    [SerializeField] private RandomTimer spawnDelay;
    [SerializeField] private Transform noTablePoint;


    [SerializeField] private Customer foodCriticPrefab;
    private Customer foodCritic;
    private bool foodCriticSpawned = false;

    private float spawnTimer;

    private static RestaurantManager instance;

    public static RestaurantManager Instance => instance;

    private List<Table> tables;

    private List<CustomerGroup> customerGroups;

    private bool isSpawnCustomer = true;

    public int initialDay;

    public CustomerSpawner Spawner => spawner;

    public Vector2 NoTablePoint => noTablePoint.position;

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

        initialDay = PlayerDataManager.Instance.currentDay;

        // Auto-load foods if not assigned in Inspector
        if (foodList == null || foodList.Length == 0)
        {
            LoadFoodsFromProject();
        }
    }
    
    private void LoadFoodsFromProject()
    {
        #if UNITY_EDITOR
        // Search for all Food ScriptableObjects in the Assets/Data/Food folder
        string[] guids = AssetDatabase.FindAssets("t:Food", new[] { "Assets/Data/Food" });
        
        if (guids.Length > 0)
        {
            foodList = new Food[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                foodList[i] = AssetDatabase.LoadAssetAtPath<Food>(path);
            }
            Debug.Log($"RestaurantManager: Loaded {foodList.Length} foods from Assets/Data/Food");
        }
        else
        {
            Debug.LogWarning("RestaurantManager: No Food assets found in Assets/Data/Food folder!");
            foodList = new Food[0];
        }
        #endif
    }

    private void Update()
    {
        SpawnCustomerCheck();

        if (isSpawnCustomer)
        {
            SpawnCustomer();
        }
    }


    public Food[] GetAvailableFoodList()
    {
        List<Food> availableFoods = new();

        foreach (Food food in foodList)
        {
            if (food == null)
                continue;

            if (CanCookFood(food))
            {
                availableFoods.Add(food);
            }
        }

        return availableFoods.ToArray();
    }


    private bool CanCookFood(Food food)
    {
        if (food == null)
            return false;

        Dictionary<ItemData, int> requiredIngredients = new();

        foreach (ItemData ingredient in food.Ingredients)
        {
            if (ingredient == null)
                continue;

            if (requiredIngredients.ContainsKey(ingredient))
            {
                requiredIngredients[ingredient]++;
            }
            else
            {
                requiredIngredients[ingredient] = 1;
            }
        }

        foreach (var pair in requiredIngredients)
        {
            if (!PlayerDataManager.Instance.HasItem(pair.Key, pair.Value))
            {
                return false;
            }
        }

        return true;
    }


    private void SpawnCustomerCheck()
    {
        if (PlayerDataManager.Instance.currentDay == PlayerDataManager.Instance.foodCriticDay && PlayerDataManager.Instance.currentDay == initialDay)
        {
            isSpawnCustomer = false;
            SpawnFoodCritic();
        }
        else if (DayCycleManager.Instance == null || !DayCycleManager.Instance.isNight)
        {
            isSpawnCustomer = false;
        }
        else
        {
            isSpawnCustomer = true;
        }
    }

    private bool SpawnCustomer()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0)
        {
            spawnTimer = spawnDelay.GetRandomDelay();

            List<Customer> customers = spawner.SpawnCustomer();

            if (customers == null) return false;

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

                c.gameObject.SetActive(true);

                c.Initialize();

            }

            customerGroups.Add(group);
        }

        return true;
    }

    private void SpawnFoodCritic()
    {
        if (foodCriticSpawned) return;
        foodCriticSpawned = true;

        foodCritic = Instantiate(foodCriticPrefab, transform.position, Quaternion.identity);

        CustomerGroup group = new CustomerGroup(new List<Customer> { foodCritic });
        Table t = AssignedTable(1);

        if (t != null)
        {
            group.AssignedTable(t);
            t.Take();
        }

        foodCritic.SetGroup(group);
        foodCritic.gameObject.SetActive(true);
        foodCritic.Initialize();

        customerGroups.Add(group);
    }

    public void RegisterTable(Table table) => tables.Add(table);

    public List<Table> GetAvailableTables(int amountOfCustomers)
    {
        return tables.FindAll(table => !table.isTaken && !table.HasDirtyDishes && table.seatAmount >= amountOfCustomers);
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
