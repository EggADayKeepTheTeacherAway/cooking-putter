using UnityEngine;
using System.Collections.Generic;

public class Player : Entity
{
    public float runSpeedModifier = 1.5f;

    public Player_IdleState idleState { get; private set; }

    public Player_MoveState moveState { get; private set; }

    private Vector2 currentVelocity;
    public Vector2 moveInput { get; private set; }

    public PlayerInputSet input { get; private set; }

    public int money = 100;

    public List<InventoryEntry> inventory = new List<InventoryEntry>();

    protected override void Awake()
    {
        base.Awake();

        input = new PlayerInputSet();

        idleState = new Player_IdleState(this, stateMachine, "idle");
        moveState = new Player_MoveState(this, stateMachine, "move");

        stateMachine.Initialize(idleState);

    }

    protected override void Update()
    {
        base.Update();
    }
    
    private void FixedUpdate()
    {
        rb.linearVelocity = currentVelocity;
    }

    private void OnEnable()
    {
        input.Enable();

        input.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Move.canceled += ctx => moveInput = Vector2.zero;

    }

    private void OnDisable()
    {
        input.Disable();
    }

    public void AddItem(ItemData item, int amount = 1)
    {
        foreach (var entry in inventory)
        {
            if (entry.item == item)
            {
                entry.quantity += amount;
                return;
            }
        }

        inventory.Add(new InventoryEntry { item = item, quantity = amount });
    }

    public void SetVelocity(float velocityX, float velocityY)
    {
        currentVelocity = new Vector2(velocityX, velocityY);
    }

}
