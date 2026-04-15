using UnityEngine;

public class Entity_IdleState : EntityState
{

    public Entity_IdleState(Entity entity, StateMachine stateMachine, string animParam) : base(entity, stateMachine, animParam)
    {
    }

    public override void Enter()
    {
        base.Enter();

        rb.linearVelocity = new Vector2(0, 0);
    }

    public override void Update()
    {
        base.Update();

        if (entity.moveInput != Vector2.zero)
        {
            stateMachine.ChangeState(entity.moveState);
        }
    }
}
