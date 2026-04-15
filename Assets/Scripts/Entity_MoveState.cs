using UnityEngine;

public class Entity_MoveState : EntityState
{
    public Entity_MoveState(Entity entity, StateMachine stateMachine, string animParam) : base(entity, stateMachine, animParam)
    {
    }

    public override void Update()
    {
        base.Update();

        if (entity.moveInput == Vector2.zero)
        {
            stateMachine.ChangeState(entity.idleState);
        }

        entity.SetVelocity(entity.moveInput.x * entity.moveSpeed, entity.moveInput.y * entity.moveSpeed);
    }
}
