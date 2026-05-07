using System.Collections.Generic;
using UnityEngine;

public class CustomerGroup
{
    public List<Customer> customers { get; private set; }
    public Table table { get; private set; }

    public System.Action OnGroupFinishedEatting;

    public int Count;

    public CustomerGroup(List<Customer> customers, Table table = null)
    {
        this.customers = customers;
        this.table = table;
        Count = customers.Count;
    }

    public void AssignedTable(Table table) => this.table = table;

    public void FinishedEatting()
    {
        Count--;

        if (Count <= 0)
        {
            // Do not UnTake here, wait until they return the seats
            OnGroupFinishedEatting?.Invoke();
        }
    }

    public void RemoveCustomer(Customer c)
    {
        if (c.seat != null)
        {
            table.ReturnSeat(c.seat);
        }
        customers.Remove(c);

        if (customers.Count == 0)
        {
            if (table != null)
            {
                table.UnTake();
            }
        }
    }
}
