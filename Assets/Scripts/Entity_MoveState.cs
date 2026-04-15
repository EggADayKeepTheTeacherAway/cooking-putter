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
        
        if (entity.input.Player.Sprint.IsPressed())
        {
            entity.SetVelocity(
                entity.moveInput.x * entity.moveSpeed * entity.runSpeedModifier,
                entity.moveInput.y * entity.moveSpeed * entity.runSpeedModifier
            );
        }

        else
        {
            entity.SetVelocity(entity.moveInput.x * entity.moveSpeed, entity.moveInput.y * entity.moveSpeed);
        }
    }
}
