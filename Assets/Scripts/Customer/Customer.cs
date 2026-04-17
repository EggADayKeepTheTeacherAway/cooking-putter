using UnityEngine;

public class Customer : Entity
{
    public Entity_IdleState idleState;
 
    protected override void Awake()
    {
        base.Awake();

        idleState = new Entity_IdleState(this, stateMachine, "idle");

        stateMachine.Initialize(idleState);
    }
}
