using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class Player : Entity
{
    [Header("Collision Detector")]
    [SerializeField] private Transform playerHead;
    [SerializeField] private float interactLine;
    [SerializeField] private LayerMask interactableObjectLayer;
    [SerializeField] private Sprite bubble;
    [SerializeField] private Transform playerFoot;

    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [Header("SFX name")]
    [SerializeField] private string roadFootStep;
    [SerializeField] private string grassFootStep;
    [SerializeField] private bool ignoredRoadCheck;
    [SerializeField] private Tilemap roadTilemap;

    public float runSpeedModifier = 1.5f;
    
    [Header("Food UI")]
    [SerializeField] private Sprite tickSprite;

    private GameObject carriedFoodPreview;
    private GameObject tickObject;


    public Player_IdleState idleState { get; private set; }

    public Player_MoveState moveState { get; private set; }

    public Vector2 currentVelocity { get; private set; }
    public Vector2 moveInput { get; private set; }

    public PlayerInputSet input { get; private set; }

    // Use PlayerDataManager instead of local storage
    public int money => PlayerDataManager.Instance.money;
    public List<InventoryEntry> inventory => PlayerDataManager.Instance.inventory;
    public BaseFood carriedFood { get; private set; }

    public AudioSource AudioSource => audioSource;

    private bool controlsLocked = false;
    protected override void Awake()
    {
        base.Awake();

        SetFacingDirection(FacingDirection.Down);

        input = new PlayerInputSet();

        idleState = new Player_IdleState(this, stateMachine, "idle");
        moveState = new Player_MoveState(this, stateMachine, "move");

        stateMachine.Initialize(idleState);

        carriedFood = null;
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

        if (tickObject != null && carriedFood != null)
        {
            tickObject.SetActive(carriedFood.IsDone());
        }

        if (controlsLocked)
            return;
        
        UpdateFacingDirection(moveInput);
        HandleInteraction();
    }

    private void LateUpdate()
    {
        sr.sortingOrder = Mathf.RoundToInt(-transform.position.y * 10);
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
            SceneTransitionManager.Instance.SetPlayerLastPosition(new Vector3(transform.position.x - 1, transform.position.y, transform.position.z));
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
        if (!input.Player.Interact.WasPressedThisFrame()) return;

        RaycastHit2D hit = ObjectDetected();
        if (hit.collider != null)
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
        foreach (var ingredient in food.Ingredients)
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

        carriedFood = new BaseFood(food);
        SpawnCarriedFoodPreview(food);
        Debug.Log($"Carrying {food.FoodName}");
    }

    public bool PickUpServiceFood(Food food)
    {
        if (food == null)
        {
            Debug.LogWarning("Cannot carry null food");
            return false;
        }

        if(carriedFood != null)
        {
            Debug.LogWarning("Already carry food");
            return false;
        }

        carriedFood = new BaseFood(food);
        carriedFood.MarkDone();
        SpawnCarriedFoodPreview(food);
        Debug.Log($"Carrying service food {food.FoodName}");
        return true;
    }

    private void SpawnCarriedFoodPreview(Food food)
    {
        carriedFoodPreview = new GameObject(food.FoodName);
        carriedFoodPreview.transform.SetParent(this.transform);
        carriedFoodPreview.transform.localPosition = new Vector3(0f, 1.5f, 0f);
        carriedFoodPreview.transform.localScale = Vector3.one * 3;

        var foodSr = carriedFoodPreview.AddComponent<SpriteRenderer>();
        foodSr.sortingLayerName = "Foreground";
        foodSr.sortingOrder = 1;
        foodSr.sprite = food.Icon;

        var bubbleObj = new GameObject($"{food.FoodName}Bubble");
        bubbleObj.transform.SetParent(carriedFoodPreview.transform);
        bubbleObj.transform.localPosition = new Vector3(0f, -0.03f, 0f);
        bubbleObj.transform.localScale = Vector3.one * 0.08f;

        var bubbleSr = bubbleObj.AddComponent<SpriteRenderer>();
        bubbleSr.sprite = bubble;
        bubbleSr.sortingLayerName = "Foreground";
        bubbleSr.sortingOrder = 0;

        tickObject = new GameObject("DoneTick");
        tickObject.transform.SetParent(carriedFoodPreview.transform);
        tickObject.transform.localPosition = new Vector3(0.031f, 0.0251f, 0f);

        tickObject.transform.localScale = new Vector3(
            0.8575131f,
            0.749206f,
            0.5905753f
        );

        var tickSr = tickObject.AddComponent<SpriteRenderer>();
        tickSr.sprite = tickSprite;
        tickSr.sortingLayerName = "Foreground";
        tickSr.sortingOrder = 2;

        tickObject.SetActive(carriedFood != null && carriedFood.IsDone());
    }

    public void DiscardFood()
    {
        if (carriedFood == null) return;

        if (carriedFoodPreview != null)
        {
            Destroy(carriedFoodPreview);
        }

        carriedFood = null;
    }

    public void PlayFootStepSound()
    {
        string soundName = IsOnRoad() || ignoredRoadCheck ? roadFootStep : grassFootStep;
        AudioManager.Instance.PlaySFX(soundName, audioSource);
    }
    
    public bool IsOnRoad()
    {
        if (ignoredRoadCheck) return true;
        if (roadTilemap == null) return false;

        Vector3Int cellPos = roadTilemap.WorldToCell(playerFoot.position);
        return roadTilemap.GetTile(cellPos) != null;
    }
}
