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

        Table table = customer.GetTable();

        if (table == null)
        {
            stateMachine.ChangeState(customer.idleState);
        }

        ChooseSeat();

        if (table.seatAmount <= 2)
        {
            customer.SetMoveTarget(RestaurantManager.Instance.RowEntryUpper);
        }

        else
        {
            customer.SetMoveTarget(RestaurantManager.Instance.RowEntryBottom);
        }


        if (seat.seatType == SeatType.Bottom)
        {
            customer.TopSortingOrder();
        }
        else if (seat.seatType == SeatType.Top)
        {
            customer.BottomSortingOrder();
        }
    }

    public override void Update()
    {
        base.Update();

        if (Vector2.Distance(customer.transform.position, customer.GetMoveTarget()) <= 0.02f)
        {
            customer.SetMoveTarget(customer.GetTable().ApproachPoint);
            //customer.SetMoveTarget(seat.GetSeatPos());
        }

    }

    public override void Exit()
    {
        base.Exit();
    }

    private void ChooseSeat() => seat = customer.GetTable().GetRandomSeat();

}
