using UnityEngine;

public class StateMachine
{
    public EntityState currentState;

    public void Initialize(EntityState initialState)
    {
        currentState = initialState;
        currentState.Enter();
    }

    public void ChangeState(EntityState newState)
    {
        Debug.Log("Are you still there");
        
        currentState.Exit();

        currentState = newState;

        currentState.Enter();

    }

    public void CallUpdateCurrentState()
    {
        currentState.Update();
    }
}
