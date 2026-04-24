using UnityEngine;
using static Player;
using static UnityEngine.EventSystems.EventTrigger;

public class PlayerState : EntityState
{
    protected Player player;
    public PlayerState(Player player, StateMachine stateMachine, string animParam) : base(player, stateMachine, animParam)
    {
        this.player = player;
    }

    public override void UpdateAnimationParameters()
    {
        anim.SetFloat("xSpeed", player.moveInput.x);
        anim.SetFloat("ySpeed", player.moveInput.y);

        anim.SetFloat("idleX", player.facingDirection == Entity.FacingDirection.Right ? 1 :
                     player.facingDirection == Entity.FacingDirection.Left ? -1 : 0);
        anim.SetFloat("idleY", player.facingDirection == Entity.FacingDirection.Up ? 1 :
                                player.facingDirection == Entity.FacingDirection.Down ? -1 : 0);
    }
}
