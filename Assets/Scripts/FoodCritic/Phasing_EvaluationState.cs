using System.Collections.Generic;
using UnityEngine;

public class Phasing_EvaluationState : CustomerState
{
    private SpriteRenderer spriteRenderer;

    private float rainbowSpeed = 2f;
    private float growSpeed = 2f;
    private float maxScale = 9f;
    private float shakeSpeed = 15f;
    private float shakeAmount = 10f;


    public Phasing_EvaluationState(
        Customer customer,
        StateMachine stateMachine,
        string animParam
    ) : base(customer, stateMachine, animParam)
    {
    }

    public override void Enter()
    {
        base.Enter();

        spriteRenderer = customer.sr;

        customer.SetPath(new List<Vector2>());
        customer.rb.linearVelocity = new Vector2(0, 0);
    }

    public override void Update()
    {
        base.Update();

        RainbowEffect();
        GrowEffect();
        ShakeEffect();
    }

    private void RainbowEffect()
    {
        float hue = Mathf.PingPong(Time.time * rainbowSpeed, 1f);

        Color rainbow = Color.HSVToRGB(hue, 1f, 1f);

        spriteRenderer.color = rainbow;
    }

    private void GrowEffect()
    {
        customer.transform.localScale +=
            Vector3.one * growSpeed * Time.deltaTime;

        if (customer.transform.localScale.x >= maxScale)
        {
            customer.gameObject.SetActive(false);
            RestaurantManager.Instance.SpawnEndLetter(customer);
        }
    }

    private void ShakeEffect()
    {
        float rotation =
            Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;

        customer.transform.rotation =
            Quaternion.Euler(0f, 0f, rotation);
    }
}