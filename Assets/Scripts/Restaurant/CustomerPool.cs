using System.Collections.Generic;
using UnityEngine;

public class CustomerPool : MonoBehaviour
{
    [SerializeField] private Customer customerPrefab;
    [SerializeField] private int amountOfCustomer = 100;

    private List<Customer> customers;

    public int AvailableCount => customers.Count;

    private void Awake()
    {
        customers = new List<Customer>();

        for (int i = 0; i < amountOfCustomer; i++)
        {
            Customer c = Instantiate(customerPrefab, transform);
            c.gameObject.SetActive(false);
            customers.Add(c);
        }
    }

    public Customer GetCustomer()
    {
        if (customers.Count <= 0)
        {
            Debug.Log("Oi Oi Oi, not enough customer in the pool dude!!!");
            Debug.Log(customers.Count);
            return null;
        }

        Customer customer = customers[customers.Count - 1];

        customers.RemoveAt(customers.Count - 1);

        return customer;
    }

    public void ReturnCustomer(Customer c)
    {
        customers.Add(c);

        c.ResetSortingOrder();

        c.gameObject.SetActive(false);

        Debug.Log(customers.Count);
    }
}
