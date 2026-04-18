using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [SerializeField] private Sprite[] allCustomerSprites;
    [SerializeField] private int maximumCustomerInGroup = 4;
    [SerializeField] private CustomerPool customerPool;

    [SerializeField] private Vector2 customerSpawnGap = new Vector2(1, 2);
    [SerializeField] private int maxPerRow = 2;


    public List<Customer> SpawnCustomer()
    {
        int totalCustomerInGroup = Random.Range(1, maximumCustomerInGroup + 1);

        List<Customer> customers = new List<Customer>();

        for (int i = 0; i < totalCustomerInGroup; i++)
        {
            Customer c = customerPool.GetCustomer();

            int column = i % maxPerRow; 
            int row = i / maxPerRow;    

            Vector3 offset = new Vector3(column * customerSpawnGap.x, -row * customerSpawnGap.y, 0);

            c.transform.position = transform.position + offset;

            customers.Add(c);
        }

        return customers;
    }

    public void ReturnCustomer(List<Customer> customers)
    {
        foreach (var c in customers)
        {
            customerPool.ReturnCustomer(c);
        }
    }
}
