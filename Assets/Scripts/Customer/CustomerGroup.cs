using System.Collections.Generic;
using UnityEngine;

public class CustomerGroup
{
    public List<Customer> customers { get; private set; }
    public Table table { get; private set; }

    public int Count => customers.Count;

    public CustomerGroup(List<Customer> customers, Table table = null)
    {
        this.customers = customers;
        this.table = table;
    }

    public void AssignedTable(Table table) => this.table = table;

}
