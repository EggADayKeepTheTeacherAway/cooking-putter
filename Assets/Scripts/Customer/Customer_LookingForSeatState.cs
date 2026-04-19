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

        if (customer.GetTable() == null)
        {
            stateMachine.ChangeState(customer.idleState);
        }

        ChooseSeat();

        if (seat.seatType == SeatTypes.Bottom)
        {
            customer.TopSortingOrder();
        }
        else if (seat.seatType == SeatTypes.Top)
        {
            customer.BottomSortingOrder();
        }
    }

    public override void Update()
    {
        base.Update();

        Vector2 direction = (seat.GetSeatPos() - (Vector2)customer.transform.position).normalized;
        rb.linearVelocity = direction * customer.moveSpeed;

    }

    public override void Exit()
    {
        base.Exit();
    }

    private void ChooseSeat() => seat = customer.GetTable().GetRandomSeat();

}
