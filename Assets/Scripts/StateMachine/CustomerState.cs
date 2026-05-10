using UnityEngine;

public class CustomerState : EntityState
{
    protected Customer customer;

    public CustomerState(Customer customer, StateMachine stateMachine, string animParam) : base(customer, stateMachine, animParam)
    {
        this.customer = customer;
    }

    public override void Enter()
    {
        base.Enter();

    }

    protected void ChooseSeat() => customer.RandomSeat();

    public override void UpdateAnimationParameters()
    {
        anim.SetFloat("facingX", customer.facingDirection == Entity.FacingDirection.Right ? 1 :
                     customer.facingDirection == Entity.FacingDirection.Left ? -1 : 0);
        anim.SetFloat("facingY", customer.facingDirection == Entity.FacingDirection.Up ? 1 :
                                customer.facingDirection == Entity.FacingDirection.Down ? -1 : 0);
    }
}
