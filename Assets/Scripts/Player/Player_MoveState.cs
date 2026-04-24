using UnityEngine;

public class Player_MoveState : PlayerState
{
    public Player_MoveState(Player player, StateMachine stateMachine, string animParam) : base(player, stateMachine, animParam)
    {
    }

    public override void Update()
    {
        base.Update();

        if (player.moveInput == Vector2.zero)
        {
            stateMachine.ChangeState(player.idleState);
        }
        
        if (player.input.Player.Sprint.IsPressed())
        {
            player.SetVelocity(
                player.moveInput.x * player.moveSpeed * player.runSpeedModifier,
                player.moveInput.y * player.moveSpeed * player.runSpeedModifier
            );
        }

        else
        {
            player.SetVelocity(player.moveInput.x * player.moveSpeed, player.moveInput.y * player.moveSpeed);
        }
    }
}
