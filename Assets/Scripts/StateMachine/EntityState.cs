using System;
using UnityEngine;

public abstract class EntityState
{
    public StateMachine stateMachine { get; private set; }

    protected float stateTimer;
    protected Animator anim;

    protected Rigidbody2D rb;

    public string animParam { get; private set; }


    protected EntityState(Entity entity, StateMachine stateMachine, string animParam)
    {
        this.stateMachine = stateMachine;
        this.animParam = animParam;

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

    }
}
