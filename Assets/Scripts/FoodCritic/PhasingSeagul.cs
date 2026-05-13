using UnityEngine;

public class PhasingSeagul : MonoBehaviour
{
    public static PhasingSeagul Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;


    }

    public void SpawnSpecialCustomer(Customer customer)
    {
        Phasing_EvaluationState evaluationState = new Phasing_EvaluationState(customer, customer.stateMachine, "idle");

        customer.stateMachine.ChangeState(evaluationState);
    }
}