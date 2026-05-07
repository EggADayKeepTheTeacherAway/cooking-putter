using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Customer_ExitState : CustomerState
{
    private Seat seat;

    public Customer_ExitState(Customer customer, StateMachine stateMachine, string animParam) : base(customer, stateMachine, animParam)
    {
    }

    public override void Enter()
    {
        base.Enter();

        customer.OnReachedTarget += OnMovementFinished;

        customer.ResetSortingOrder();

        WalkToExit();
    }

    public override void Exit()
    {
        base.Exit();

        customer.OnReachedTarget -= OnMovementFinished;
        customer.GetGroup().RemoveCustomer(customer);
    }

    private void WalkToExit()
    {
        Table table = customer.GetTable();

        if (table == null)
        {
            stateMachine.ChangeState(customer.idleState);
            return;
        }

        seat = customer.seat;

        List<Vector2> path = BuildPath(table);

        customer.SetPath(path);
    }

    private void OnMovementFinished() => RestaurantManager.Instance.Spawner.ReturnCustomer(customer);

    private List<Vector2> BuildPath(Table table)
    {
        float exitOffset = 2f;
        CustomerSpawner spawner = RestaurantManager.Instance.Spawner;

        List<Vector2> path = new List<Vector2>();

        Vector2 seatPos = seat.GetSeatPos();

        path.Add(new Vector2(table.ApproachPoint.x, seatPos.y));

        path.Add(new Vector2(table.ApproachPoint.x, table.ApproachPoint.y));

        path.Add(new Vector2(spawner.transform.position.x, table.ApproachPoint.y));

        path.Add(new Vector2(spawner.transform.position.x, spawner.transform.position.y - exitOffset));


        return path;
    }
}
