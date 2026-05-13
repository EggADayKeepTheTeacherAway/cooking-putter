using System.Collections.Generic;
using UnityEngine;

public class Customer_ExitState : CustomerState
{
    private Seat seat;

    private float phasinTimer = 2;

    public Customer_ExitState(Customer customer, StateMachine stateMachine, string animParam) : base(customer, stateMachine, animParam)
    {
    }

    public override void Enter()
    {
        base.Enter();

        seat = null;

        customer.OnReachedTarget += OnMovementFinished;

        customer.ResetSortingOrder();

        stateTimer = phasinTimer;

        WalkToExit();
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer <= 0)
        {
            PhasingSeagul.Instance?.SpawnSpecialCustomer(customer);
        }

    }

    private void WalkToExit()
    {
        float exitOffset = 2f;

        Table table = customer.GetTable();

        List<Vector2> path;

        if (table == null || customer.seat == null)
        {
            CustomerSpawner spawner = RestaurantManager.Instance.Spawner;
 
            path = new List<Vector2>();

            path.Add(new Vector2(customer.transform.position.x, spawner.transform.position.y - exitOffset));
        }
        else
        {
            seat = customer.seat;

            path = BuildPath(table);
        }

        customer.SetPath(path);
    }

    private void OnMovementFinished()
    {
        customer.OnReachedTarget -= OnMovementFinished;
        CustomerGroup group = customer.GetGroup();

        if (group != null)
        {
            group.RemoveCustomer(customer);
        }

        RestaurantManager.Instance.Spawner.ReturnCustomer(customer);
    }

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
