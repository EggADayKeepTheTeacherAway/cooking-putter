using System;
using UnityEngine;

public abstract class EntityState
{
    public StateMachine stateMachine { get; private set; }

    private Entity entity;

    protected float stateTimer;
    protected Animator anim;

    protected Rigidbody2D rb;

    public string animParam { get; private set; }


    protected EntityState(Entity entity, StateMachine stateMachine, string animParam)
    {
        this.stateMachine = stateMachine;
        this.animParam = animParam;

        this.entity = entity;
        this.anim = entity.anim;
        this.rb = entity.rb;

    }

    public virtual void Enter()
    {
        anim.SetBool(animParam, true);
    }

    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;

        UpdateAnimationParameters();
    }

    public virtual void Exit()
    {
        anim.SetBool(animParam, false);
    }

    public virtual void UpdateAnimationParameters()
    {

        anim.SetFloat("idleX", entity.facingDirection == Entity.FacingDirection.Right ? 1 :
                        entity.facingDirection == Entity.FacingDirection.Left ? -1 : 0);
        anim.SetFloat("idleY", entity.facingDirection == Entity.FacingDirection.Up ? 1 :
                                entity.facingDirection == Entity.FacingDirection.Down ? -1 : 0);
    }
}
