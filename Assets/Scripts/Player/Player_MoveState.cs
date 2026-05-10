using UnityEngine;

public class Player_MoveState : PlayerState
{
    private float walkingAnimationSpeed = 1f;
    private float runningAnimationSpeed = 2.5f;


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
            anim.SetFloat("speedMultiplier", runningAnimationSpeed);
            player.SetVelocity(
                player.moveInput.x * player.moveSpeed * player.runSpeedModifier,
                player.moveInput.y * player.moveSpeed * player.runSpeedModifier
            );

        }

        else
        {
            anim.SetFloat("speedMultiplier", walkingAnimationSpeed);
            player.SetVelocity(player.moveInput.x * player.moveSpeed, player.moveInput.y * player.moveSpeed);
        }
    }

    public override void Exit()
    {
        base.Exit();

        anim.SetFloat("speedMultiplier", walkingAnimationSpeed);
    }
}
