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

        customer.OnReachedTarget += OnMovementFinished;

        Table table = customer.GetTable();

        if (table == null)
        {
            stateMachine.ChangeState(customer.idleState);
            return;
        }

        ChooseSeat();

        List<Vector2> path = BuildPath(table, seat);

        customer.SetPath(path);

    }

    public override void Update()
    {
        base.Update();
        
    }

    public override void Exit()
    {
        base.Exit();

        customer.OnReachedTarget -= OnMovementFinished;
    }

    private void OnMovementFinished()
    {
        if (seat.seatType == SeatType.Bottom)
        {
            customer.SetFacingDirection(Entity.FacingDirection.Up);
            customer.SetTopSortingOrder();
        }
        else if (seat.seatType == SeatType.Top)
        {
            customer.SetFacingDirection(Entity.FacingDirection.Down);
            customer.SetBottomSortingOrder();
        }

        stateMachine.ChangeState(customer.waitFoodState);
    }

    private void ChooseSeat() => seat = customer.GetTable().GetRandomSeat();

    private List<Vector2> BuildPath(Table table, Seat seat)
    {
        float seatOffsetX = 1f;
        float bottomSeatOffset = 0.5f;

        List<Vector2> path = new List<Vector2>();

        Vector2 seatPos = seat.GetSeatPos();

        path.Add(new Vector2(customer.transform.position.x, table.ApproachPoint.y));


        if (seat.approachSide == ApproachSide.Left) seatOffsetX = -seatOffsetX;

        // Walking to the side of seat
        path.Add(new Vector2(seatPos.x + seatOffsetX, table.ApproachPoint.y));


        if (seat.seatType == SeatType.Top) // if seat top go to the top 
        {
            path.Add(new Vector2(seatPos.x + seatOffsetX, seatPos.y));
        }

        if (seat.seatType == SeatType.Bottom)
        {
            path.Add(new Vector2(seatPos.x, seatPos.y + bottomSeatOffset));
        }

        else if (seat.seatType == SeatType.Top)
        {
            path.Add(seatPos);
        }
        return path;
    }
}
