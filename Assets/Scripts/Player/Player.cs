using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Entity
{
    [SerializeField] private Transform playerHead;
    [SerializeField] private float interactLine;
    [SerializeField] private LayerMask interactableObjectLayer;

    public float runSpeedModifier = 1.5f;

    public Player_IdleState idleState { get; private set; }

    public Player_MoveState moveState { get; private set; }

    public Vector2 currentVelocity { get; private set; }
    public Vector2 moveInput { get; private set; }

    public PlayerInputSet input { get; private set; }

    // Use PlayerDataManager instead of local storage
    public int money => PlayerDataManager.Instance.money;
    public List<InventoryEntry> inventory => PlayerDataManager.Instance.inventory;
    private bool controlsLocked = false;
    protected override void Awake()
    {
        base.Awake();

        SetFacingDirection(FacingDirection.Down);

        input = new PlayerInputSet();

        idleState = new Player_IdleState(this, stateMachine, "idle");
        moveState = new Player_MoveState(this, stateMachine, "move");

        stateMachine.Initialize(idleState);
    }
    public void LockControls()
    {
        controlsLocked = true;

        moveInput = Vector2.zero;
        currentVelocity = Vector2.zero;

        rb.linearVelocity = Vector2.zero;
    }

    public void UnlockControls()
    {
        controlsLocked = false;
    }
    protected override void Update()
    {
        base.Update();

        if (controlsLocked)
            return;
        UpdateFacingDirection(moveInput);
        HandleInteraction();
    }
    
    private void FixedUpdate()
    {
        if (controlsLocked)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        rb.linearVelocity = currentVelocity;
    }

    private void OnEnable()
    {
        input.Enable();

        input.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneTransitionManager.Instance.OnSceneTransitioning += OnChangeScene;

    }

    private void OnDisable()
    {
        input.Disable();

        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneTransitionManager.Instance.OnSceneTransitioning -= OnChangeScene;

    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "TownScene")
        {
            // New-day forced spawn
            if (SceneTransitionManager.Instance.overrideSpawnPosition)
            {
                transform.position =
                    SceneTransitionManager.Instance.forcedSpawnPosition;

                SceneTransitionManager.Instance.overrideSpawnPosition = false;
            }
            else
            {
                // Normal transition
                transform.position =
                    SceneTransitionManager.Instance.playerLastPosition;
            }
        }
    }

    private void OnChangeScene(string sceneName)
    {
        if (sceneName != "TownScene")
        {
            SceneTransitionManager.Instance.SetPlayerLastPosition(transform.position);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.lightGreen;
        Gizmos.DrawLine(playerHead.position, playerHead.position + ConvertFacingDirToVector() * interactLine);
    }

    protected RaycastHit2D ObjectDetected()
    {
        return Physics2D.Raycast(playerHead.position, ConvertFacingDirToVector(), interactLine, interactableObjectLayer);
    }

    private void HandleInteraction()
    {
        RaycastHit2D hit = ObjectDetected();
        if (hit.collider != null && input.Player.Interact.WasPressedThisFrame())
        {
            hit.collider.GetComponent<IInteractable>()?.Interact();
        }
    }

    public void AddItem(ItemData item, int amount = 1)
    {
        PlayerDataManager.Instance.AddItem(item, amount);
    }

    public void SetVelocity(float velocityX, float velocityY)
    {
        currentVelocity = new Vector2(velocityX, velocityY);
    }

    private void OnApplicationQuit()
    {
        PlayerDataManager.Instance.SaveData();
    }
}
