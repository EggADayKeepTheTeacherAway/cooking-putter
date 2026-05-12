using UnityEngine;

public class Player_MoveState : PlayerState
{
    private float footstepTimer = 0f;
    private float walkFootstepInterval = 0.2f;
    private float runFootstepInterval = 2f;

    private float walkingAnimationSpeed = 1f;
    private float runningAnimationSpeed = 2.5f;

    private bool isStopping = false;


    public Player_MoveState(Player player, StateMachine stateMachine, string animParam) : base(player, stateMachine, animParam)
    {
    }

    public override void Enter()
    {
        base.Enter();

        isStopping = false;

    }

    public override void Update()
    {
        base.Update();

        if (player.moveInput == Vector2.zero)
        {
            player.AudioSource.Stop();

            stateMachine.ChangeState(player.idleState);
        }
        
        if (player.input.Player.Sprint.IsPressed())
        {
            anim.SetFloat("speedMultiplier", runningAnimationSpeed);
            player.SetVelocity(
                player.moveInput.x * player.moveSpeed * player.runSpeedModifier,
                player.moveInput.y * player.moveSpeed * player.runSpeedModifier
            );

            HandleFootstep(runFootstepInterval);


        }

        else
        {
            anim.SetFloat("speedMultiplier", walkingAnimationSpeed);
            player.SetVelocity(player.moveInput.x * player.moveSpeed, player.moveInput.y * player.moveSpeed);
            HandleFootstep(walkFootstepInterval);

        }
    }

    public override void Exit()
    {
        base.Exit();

        isStopping = true;
        footstepTimer = 0f;
        player.AudioSource.Stop();
        player.AudioSource.clip = null;
        anim.SetFloat("speedMultiplier", walkingAnimationSpeed);
    }

    private void HandleFootstep(float interval)
    {
        if (isStopping) return;

        footstepTimer -= Time.deltaTime;
        if (footstepTimer <= 0f)
        {
            footstepTimer = interval;
            player.PlayFootStepSound();
        }
    }
}
