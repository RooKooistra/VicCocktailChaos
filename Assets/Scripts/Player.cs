using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent
{
    public static Player Instance { get; private set; }

    public event Action<BaseCounter> OnSelectedCounterChange;
    public event Action<Transform> OnPickedSomething;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 5f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private float interactDistance = 2f;
    [SerializeField] private float playerRadius = 0.7f;
    [SerializeField] private float playerHeight = 2f;
    [SerializeField] private LayerMask countersLayerMask; //change to interactable after demo
    [SerializeField] private Transform kitchenObjectHoldPoint;

    private KitchenObject kitchenObject;


    private bool isWalking;
    private Vector3 lastInteractDirection = Vector3.zero;
    private BaseCounter selectedCounter;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"There is more than one {Instance.name}! {transform}  -  {Instance}");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
        gameInput.OnInteractSecondAction += GameInput_OnInteractSecondAction;
    }

    private void OnDisable()
    {
        gameInput.OnInteractAction -= GameInput_OnInteractAction;
        gameInput.OnInteractSecondAction -= GameInput_OnInteractSecondAction;
    }

    private void Update()
    {
        HandleMovement();
        HandleInteractions();
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
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

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
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDirection = new Vector3(inputVector.x, 0f, inputVector.y);

        #region collisions with code
        // The following is for collisions but I dont know why I wouldnt use a capsule collider. May remove this later
        float moveDistance = moveSpeed * Time.deltaTime;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirection, moveDistance);
        if (!canMove)
        {
            // Cannot move towards moveDirection

            // Attempt in X direction
            Vector3 moveDirectionX = new Vector3(moveDirection.x, 0, 0).normalized;
            canMove = moveDirection.x != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirectionX, moveDistance);
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
                canMove = moveDirection.z != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirectionZ, moveDistance);
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
}
     