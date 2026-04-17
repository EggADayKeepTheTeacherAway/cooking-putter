using UnityEngine;

public class PlayerState : EntityState
{
    protected Player player;
    public PlayerState(Player player, StateMachine stateMachine, string animParam) : base(player, stateMachine, animParam)
    {
        this.player = player;
    }

 
}
