using System.Collections.Generic;
using UnityEngine;

public class CustomerPool : MonoBehaviour
{
    [SerializeField] private Customer customerPrefab;
    [SerializeField] private int amountOfCustomer = 100;

    private List<Customer> customers;

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
        Customer customer = customers[customers.Count - 1];

        customers.RemoveAt(customers.Count - 1);

        customer.gameObject.SetActive(true);

        return customer;
    }

    public void ReturnCustomer(Customer c)
    {
        c.gameObject.SetActive(false);

        customers.Add(c);
    }
}
