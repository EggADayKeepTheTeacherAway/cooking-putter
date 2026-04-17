using UnityEngine;

public class CustomerState : EntityState
{
    protected Customer customer;
    public CustomerState(Customer customer, StateMachine stateMachine, string animParam) : base(customer, stateMachine, animParam)
    {
        this.customer = customer;
    }

}
