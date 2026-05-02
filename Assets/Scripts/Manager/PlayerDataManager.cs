using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    private static PlayerDataManager instance;
    public static PlayerDataManager Instance => instance;
    
    public int money = 1000;
    public List<InventoryEntry> inventory = new List<InventoryEntry>();

    private bool hasInitialized = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Only load data on first initialization
        if (!hasInitialized)
        {
            LoadData();
            hasInitialized = true;
            Debug.Log("PlayerDataManager initialized and data loaded");
        }
    }

    public void AddItem(ItemData item, int amount = 1)
    {
        foreach (var entry in inventory)
        {
            if (entry.item == item)
            {
                entry.quantity += amount;
                return;
            }
        }

        inventory.Add(new InventoryEntry { item = item, quantity = amount });
    }

    public void RemoveItem(ItemData item, int amount = 1)
    {
        foreach (var entry in inventory)
        {
            if (entry.item == item)
            {
                entry.quantity -= amount;
                if (entry.quantity <= 0)
                {
                    inventory.Remove(entry);
                }
                return;
            }
        }
    }

    public bool HasItem(ItemData item, int amount = 1)
    {
        foreach (var entry in inventory)
        {
            if (entry.item == item && entry.quantity >= amount)
            {
                return true;
            }
        }
        return false;
    }

    public void AddMoney(int amount)
    {
        money += amount;
    }

    public bool TrySpendMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            return true;
        }
        return false;
    }

    // Save to PlayerPrefs (permanent save)
    public void SaveData()
    {
        PlayerPrefs.SetInt("PlayerMoney", money);
        string inventoryJson = JsonUtility.ToJson(new InventoryData { items = inventory });
        PlayerPrefs.SetString("PlayerInventory", inventoryJson);
        
        PlayerPrefs.Save();
        Debug.Log("Player data saved to disk!");
    }

    // Load from PlayerPrefs (restore from permanent save)
    public void LoadData()
    {
        money = PlayerPrefs.GetInt("PlayerMoney", 1000);
        string inventoryJson = PlayerPrefs.GetString("PlayerInventory", "");
        if (!string.IsNullOrEmpty(inventoryJson))
        {
            try
            {
                InventoryData data = JsonUtility.FromJson<InventoryData>(inventoryJson);
                inventory = data.items ?? new List<InventoryEntry>();
            }
            catch
            {
                Debug.LogWarning("Failed to load inventory data, starting fresh");
                inventory = new List<InventoryEntry>();
            }
        }
        
        Debug.Log($"Player data loaded - Money: {money}, Items: {inventory.Count}");
    }

    // Clear all saved data
    public void ClearSaveData()
    {
        PlayerPrefs.DeleteKey("PlayerMoney");
        PlayerPrefs.DeleteKey("PlayerInventory");
        PlayerPrefs.Save();
        
        money = 1000;
        inventory.Clear();
        hasInitialized = false;
        Debug.Log("All save data cleared!");
    }
}
