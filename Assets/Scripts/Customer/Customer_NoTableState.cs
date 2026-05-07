using System.Collections.Generic;
using UnityEngine;

public class Customer_NoTableState : CustomerState
{
    private bool isLooking;
    private float lookingTime = 0.25f;
    public Customer_NoTableState(Customer customer, StateMachine stateMachine, string animParam) : base(customer, stateMachine, animParam)
    {
    }

    public override void Enter()
    {
        base.Enter();

        isLooking = false;

        List<Vector2> path = new List<Vector2>();

        path.Add(new Vector2(customer.transform.position.x, RestaurantManager.Instance.NoTablePoint.y));

        customer.SetPath(path);

        customer.OnReachedTarget += OnMoveFinished;
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer <= 0 && isLooking)
        {
            stateMachine.ChangeState(customer.exitState);
        }
    }

    public override void Exit()
    {
        base.Exit();

        customer.OnReachedTarget -= OnMoveFinished;

    }

    public void OnMoveFinished()
    {
        customer.SetFacingDirection(Entity.FacingDirection.Left);

        stateTimer = lookingTime;
        isLooking = true;

    }
}
