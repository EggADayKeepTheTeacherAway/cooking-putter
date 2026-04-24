using UnityEngine;
using static Player;

public class CustomerState : EntityState
{
    protected Customer customer;
    public CustomerState(Customer customer, StateMachine stateMachine, string animParam) : base(customer, stateMachine, animParam)
    {
        this.customer = customer;
    }

    public override void UpdateAnimationParameters()
    {
        anim.SetFloat("xSpeed", rb.linearVelocity.x);
        anim.SetFloat("ySpeed", rb.linearVelocity.y);
    }
}
