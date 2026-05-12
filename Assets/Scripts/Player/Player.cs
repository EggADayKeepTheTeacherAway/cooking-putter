using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Entity
{
    [SerializeField] private Transform playerHead;
    [SerializeField] private float interactLine;
    [SerializeField] private LayerMask interactableObjectLayer;
    [SerializeField] private Sprite bubble;

    public float runSpeedModifier = 1.5f;

    public Player_IdleState idleState { get; private set; }

    public Player_MoveState moveState { get; private set; }

    public Vector2 currentVelocity { get; private set; }
    public Vector2 moveInput { get; private set; }

    public PlayerInputSet input { get; private set; }

    // Use PlayerDataManager instead of local storage
    public int money => PlayerDataManager.Instance.money;
    public List<InventoryEntry> inventory => PlayerDataManager.Instance.inventory;
    public Food carriedFood { get; private set; }

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

    public void PickUpFood(Food food)
    {
        if(carriedFood != null)
        {
            Debug.LogWarning("Already carry food");
            return;
        }

        var required = new Dictionary<ItemData, int>();
        foreach (var ingredient in food.ingredients)
        {
            if (required.ContainsKey(ingredient)) required[ingredient]++;
            else required[ingredient] = 1;
        }

        foreach (var item in required)
        {
            if (!PlayerDataManager.Instance.HasItem(item.Key, item.Value))
            {
                Debug.LogWarning("Not enough ingredient");
                return;
            }
        }

        foreach (var item in required)
        {
            PlayerDataManager.Instance.RemoveItem(item.Key, item.Value);
        }

        carriedFood = food;
        var previewObj = new GameObject(food.FoodName);
        previewObj.transform.SetParent(this.transform);
        previewObj.transform.localPosition = new Vector3(0f, 1.5f, 0f);
        previewObj.transform.localScale = Vector3.one * 3;

        var foodSr = previewObj.AddComponent<SpriteRenderer>();
        foodSr.sortingLayerName = "Foreground";
        foodSr.sortingOrder = 1;
        foodSr.sprite = food.Icon;

        var bubbleObj = new GameObject($"{food.FoodName}Bubble");
        bubbleObj.transform.SetParent(previewObj.transform);
        bubbleObj.transform.localPosition = new Vector3(0f, -0.03f, 0f);
        bubbleObj.transform.localScale = Vector3.one * 0.08f;
        
        var bubbleSr = bubbleObj.AddComponent<SpriteRenderer>();
        bubbleSr.sprite = bubble;
        bubbleSr.sortingLayerName = "Foreground";
        Debug.Log($"Carrying {food.name}");
    }
}
