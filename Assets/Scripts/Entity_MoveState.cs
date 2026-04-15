using UnityEngine;

public class Entity_MoveState : EntityState
{
    public Entity_MoveState(Entity entity, StateMachine stateMachine, string animParam) : base(entity, stateMachine, animParam)
    {
    }
}
