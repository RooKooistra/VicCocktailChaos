using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour, IKitchenObjectParent
{
    public static event Action OnAnyPlayerSpawned;
    public static event Action<Transform> OnAnyPickedSomething;

    public static void ResetStaticData()
    {
        OnAnyPlayerSpawned = null;
        OnAnyPickedSomething = null;
    }
    public static Player LocalInstance { get; private set; }

    public event Action<BaseCounter> OnSelectedCounterChange;
    public event Action<Transform> OnPickedSomething;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 5f;
    // [SerializeField] private GameInput gameInput;
    [SerializeField] private float interactDistance = 2f;
    [SerializeField] private float playerRadius = 0.7f;
    //[SerializeField] private float playerHeight = 2f;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private LayerMask collisionsLayerMask; // set player layer mask to Players for player collisions
    [SerializeField] private Transform kitchenObjectHoldPoint;
    [SerializeField] private PlayerVisual playerVisual;

    [SerializeField] private List<Vector3> spawnPoints = new List<Vector3>(); // seperate players on spawn

    private KitchenObject kitchenObject;


    private bool isWalking;
    private Vector3 lastInteractDirection = Vector3.zero;
    private BaseCounter selectedCounter;

    /*
    private void Awake()
    {
        /OBSOLETE SINCE MULTIPLAYER
        if (Instance != null)
        {
            Debug.LogError($"There is more than one {Instance.name}! {transform}  -  {Instance}");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
    }
    */

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
        }

        transform.position = spawnPoints[GameMultiplayer.Instance.GetPlayerDataIndexFromClientID(OwnerClientId)];
        OnAnyPlayerSpawned?.Invoke();

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }      
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnectCallback;
        }
    }

    private void Start()
    {
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
        GameInput.Instance.OnInteractAltAction += GameInput_OnInteractSecondAction;

        PlayerData playerData = GameMultiplayer.Instance.GetPlayerDataFromClientID(OwnerClientId);
        playerVisual.SetPlayerColour(GameMultiplayer.Instance.GetPlayerColor(playerData.colourId));
    }

    private void OnDisable()
    {
        GameInput.Instance.OnInteractAction -= GameInput_OnInteractAction;
        GameInput.Instance.OnInteractAltAction -= GameInput_OnInteractSecondAction;
    }

    private void Update()
    {
        if (!IsOwner) return;

        HandleMovement();
        HandleInteractions();
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        if(clientId == OwnerClientId && HasKitchenObject())
        {
            KitchenObject.DestroyKitchenObject(GetKitchenObject());
        }
    }

    private void GameInput_OnInteractAction()
    {
        if (!GameManager.Instance.IsGamePlaying()) return;

        if (selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }
    }

    private void GameInput_OnInteractSecondAction()
    {
        if (!GameManager.Instance.IsGamePlaying()) return;

        if (selectedCounter != null)
        {
            selectedCounter.InteractSecond(this);
        }
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    private void HandleInteractions()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();

        Vector3 moveDirection = new Vector3(inputVector.x, 0f, inputVector.y);

        // keeps the interaction detection active after no direction input is received
        if(moveDirection != Vector3.zero )
        {
            lastInteractDirection = moveDirection;
        }

        if(Physics.Raycast(transform.position, lastInteractDirection, out RaycastHit raycastHit, interactDistance, countersLayerMask))
        {
            if(raycastHit.transform.TryGetComponent(out BaseCounter baseCounter)) // using inheritance but consider switching to another interface
            {
                // Has interactable
                if(baseCounter != this.selectedCounter)
                {
                    this.selectedCounter = baseCounter;

                    SetSelectedCounter(this.selectedCounter);
                }
            } 
            else
            {
                // not an interactable
                SetSelectedCounter(null);
            }
        }
        else
        {
            // nothing in front of the player
            SetSelectedCounter(null);
        }
    }

    private void HandleMovement()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();

        Vector3 moveDirection = new Vector3(inputVector.x, 0f, inputVector.y);

        #region collisions with code
        // The following is for collisions with raycasts but may switch to a capsule colliders.

        float moveDistance = moveSpeed * Time.deltaTime;
        bool canMove = !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirection, Quaternion.identity, moveDistance, collisionsLayerMask);

        if (!canMove)
        {
            // Cannot move towards moveDirection

            // Attempt in X direction
            Vector3 moveDirectionX = new Vector3(moveDirection.x, 0, 0).normalized;
            canMove = (moveDirection.x < -0.5f || moveDirection.x > 0.5f) && !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirectionX, Quaternion.identity, moveDistance, collisionsLayerMask);
            if (canMove)
            {
                // can move in the X direction
                moveDirection = moveDirectionX;
            }
            else
            {
                // can  not move in the X direction

                // Attempt in Z direction
                Vector3 moveDirectionZ = new Vector3(0, 0, moveDirection.z).normalized;
                canMove = (moveDirection.z < -0.5f || moveDirection.z > 0.5f) && !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirectionZ, Quaternion.identity, moveDistance, collisionsLayerMask);
                if (canMove)
                {
                    // can move in the Z direction
                    moveDirection = moveDirectionZ;
                }
            }
        }

        if (canMove)
        {
            transform.position += moveDirection * Time.deltaTime * moveSpeed;
        }
        #endregion

        // for animation bool
        isWalking = moveDirection != Vector3.zero;

        // rotate player
        transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
    }

    // change to an interface
    private void SetSelectedCounter(BaseCounter baseCounter)
    {
        this.selectedCounter = baseCounter;

        OnSelectedCounterChange?.Invoke(this.selectedCounter);
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        // player receives a kitchen object
        this.kitchenObject = kitchenObject;

        // event for audio
        if (kitchenObject == null) return;
        OnPickedSomething?.Invoke(transform);
        OnAnyPickedSomething?.Invoke(transform);
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        this.kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return this.kitchenObject != null;
    }

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }
}
     