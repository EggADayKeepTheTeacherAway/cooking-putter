using System.Collections.Generic;
using UnityEngine;

public class Customer_LookingForSeatState : CustomerState
{
    private Seat seat;

    public Customer_LookingForSeatState(Customer customer, StateMachine stateMachine, string animParam) : base(customer, stateMachine, animParam)
    {
    }

    public override void Enter()
    {
        base.Enter();

        ChooseSeat();
    }

    public override void Update()
    {
        base.Update();

        Vector2 direction = (seat.GetSeatPos() - (Vector2)customer.transform.position).normalized;
        rb.linearVelocity = direction * customer.moveSpeed;

    }

    private void ChooseSeat() => seat = customer.GetTable().GetRandomSeat();

}
