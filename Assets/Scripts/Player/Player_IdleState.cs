using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Player_IdleState : PlayerState
{
    public Player_IdleState(Player player, StateMachine stateMachine, string animParam) : base(player, stateMachine, animParam)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.SetVelocity(0, 0);
    }

    public override void Update()
    {
        base.Update();

        if (player.moveInput != Vector2.zero)
        {
            stateMachine.ChangeState(player.moveState);
        }
    }
}
