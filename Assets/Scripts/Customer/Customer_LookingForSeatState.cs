using System.Collections.Generic;
using UnityEngine;

public class Customer_LookingForSeatState : CustomerState
{
    private Table table;
    private Seat seat;

    public Customer_LookingForSeatState(Customer customer, StateMachine stateMachine, string animParam) : base(customer, stateMachine, animParam)
    {
    }

    public override void Enter()
    {
        base.Enter();

        ChooseTable();

        ChooseSeat();
    }

    public override void Update()
    {
        base.Update();

        Vector2 direction = (seat.GetSeatPos() - (Vector2)customer.transform.position).normalized;
        rb.linearVelocity = direction * customer.moveSpeed;

    }

    private void ChooseTable()
    {
        List<Table> notTakenTables = RestaurantManager.Instance.GetAvailableTables();

        if (notTakenTables.Count == 0)
            return;

        int chosenTableIndex = Random.Range(0, notTakenTables.Count);

        table = notTakenTables[chosenTableIndex];

        table.Take();
    }

    private void ChooseSeat() => seat = table.GetRandomSeat();

}
