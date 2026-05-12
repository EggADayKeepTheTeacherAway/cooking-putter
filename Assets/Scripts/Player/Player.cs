using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Entity
{
    [Header("Collision Detector")]
    [SerializeField] private Transform playerHead;
    [SerializeField] private float interactLine;
    [SerializeField] private LayerMask interactableObjectLayer;

    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [Header("SFX name")]
    [SerializeField] private string roadFootStep;
    [SerializeField] private string grassFootStep;

    public float runSpeedModifier = 1.5f;

    public Player_IdleState idleState { get; private set; }

    public Player_MoveState moveState { get; private set; }

    public Vector2 currentVelocity { get; private set; }
    public Vector2 moveInput { get; private set; }

    public PlayerInputSet input { get; private set; }

    // Use PlayerDataManager instead of local storage
    public int money => PlayerDataManager.Instance.money;
    public List<InventoryEntry> inventory => PlayerDataManager.Instance.inventory;

    public AudioSource AudioSource => audioSource;

    protected override void Awake()
    {
        base.Awake();

        SetFacingDirection(FacingDirection.Down);

        input = new PlayerInputSet();

        idleState = new Player_IdleState(this, stateMachine, "idle");
        moveState = new Player_MoveState(this, stateMachine, "move");

        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();

        UpdateFacingDirection(moveInput);
        HandleInteraction();
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
            transform.position = SceneTransitionManager.Instance.playerLastPosition;
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

    public void PlayFootStepSound() => AudioManager.Instance.PlaySFX(grassFootStep, audioSource);

}
