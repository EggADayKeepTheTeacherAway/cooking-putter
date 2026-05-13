using UnityEngine;

public class Phasing_EvaluationState : CustomerState
{
    private const string ANIM_PARAM_BOOM = "boom";

    private SpriteRenderer spriteRenderer;

    private float rainbowSpeed = 2f;
    private float growSpeed = 1.5f;
    private float maxScale = 3f;

    private bool exploded = false;

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

        spriteRenderer = customer.GetComponent<SpriteRenderer>();
    }

    public override void Update()
    {
        base.Update();

        if (exploded)
            return;

        RainbowEffect();
        GrowEffect();
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
            exploded = true;

            customer.anim.SetBool(ANIM_PARAM_BOOM, true);
        }
    }
}