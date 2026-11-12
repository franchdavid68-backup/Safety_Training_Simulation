using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [Header("Movement & Transition Settings")]
    [SerializeField] private bool smoothTransition = true;
    [SerializeField] private float transitionSpeed = 10f;
    [SerializeField] private float transitionRotationSpeed = 500f;
    [SerializeField] private float stepSize = 1f;

    [Header("Collision & Raycast Settings")]
    [SerializeField] private LayerMask obstacleMask;    
    [SerializeField] private float rayHeight = 0.5f;    
    [Tooltip("Half-extents of the collision box (Width/2, Height/2, Depth/2).")]
    [SerializeField] private Vector3 boxHalfExtents = new Vector3(0.4f, 0.8f, 0.1f);

    [Header("Mobile Touch Buttons")]
    [SerializeField] private TouchMovementButton forwardButton;
    [SerializeField] private TouchMovementButton backwardButton;
    [SerializeField] private TouchMovementButton turnLeftButton;
    [SerializeField] private TouchMovementButton turnRightButton;

    [Header("Bounds Settings")]
    [SerializeField] private bool useBounds = false;
    [SerializeField] private Vector2 minXZ = new Vector2(-10, -10);
    [SerializeField] private Vector2 maxXZ = new Vector2(10, 10);

    private Vector3 targetPosition;
    private Vector3 targetRotation;
    private Vector3 prevTargetPosition;

    private bool AtRest
    {
        get
        {
            return Vector3.Distance(transform.position, targetPosition) < 0.045f &&
                   Vector3.Distance(transform.eulerAngles, targetRotation) < 0.05f;
        }
    }

    void Start()
    {
        targetPosition = transform.position;
        prevTargetPosition = transform.position;
        targetRotation = transform.eulerAngles;
        if(UIState.IsBlocking){
            UIState.Pop();
        }
    }

    void Update()
    {
        if (UIState.IsBlocking)
            return;

        if (!AtRest)
            return;

        var keyboard = Keyboard.current;

        bool moveForward = (keyboard != null && keyboard.wKey.wasPressedThisFrame) || (forwardButton != null && forwardButton.WasPressedThisFrame());
        bool moveBackward = (keyboard != null && keyboard.sKey.wasPressedThisFrame) || (backwardButton != null && backwardButton.WasPressedThisFrame());
        
        bool turnLeft = (keyboard != null && keyboard.qKey.wasPressedThisFrame) || (turnLeftButton != null && turnLeftButton.WasPressedThisFrame());
        bool turnRight = (keyboard != null && keyboard.eKey.wasPressedThisFrame) || (turnRightButton != null && turnRightButton.WasPressedThisFrame());

        if (turnLeft)
        {
            targetRotation -= new Vector3(0f, 90f, 0f);
            NormalizeRotation();
        }
        else if (turnRight)
        {
            targetRotation += new Vector3(0f, 90f, 0f);
            NormalizeRotation();
        }

        Vector3 moveDirection = Vector3.zero;
        if (moveForward)        moveDirection = transform.forward;
        else if (moveBackward)  moveDirection = -transform.forward;

        if (moveDirection != Vector3.zero)
            TryMove(moveDirection);
    }

    void FixedUpdate()
    {
        if (smoothTransition)
        {
            // Smoothly interpolate position and rotation
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.fixedDeltaTime * transitionSpeed);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(targetRotation), Time.fixedDeltaTime * transitionRotationSpeed);
        }
        else
        {
            transform.position = targetPosition;
            transform.rotation = Quaternion.Euler(targetRotation);
        }
    }

    private void TryMove(Vector3 direction)
    {
        direction = new Vector3(Mathf.Round(direction.x), 0f, Mathf.Round(direction.z));

        Vector3 desiredPosition = transform.position + direction * stepSize;
        desiredPosition.x = Mathf.Round(desiredPosition.x / stepSize) * stepSize;
        desiredPosition.z = Mathf.Round(desiredPosition.z / stepSize) * stepSize;

        Vector3 origin = transform.position + Vector3.up * rayHeight;
        float castDistance = stepSize + 0.2f;

        // Check for obstacles using BoxCast
        if (Physics.BoxCast(origin, boxHalfExtents, direction, transform.rotation, castDistance, obstacleMask))
        {
            targetPosition = prevTargetPosition; // Revert to previous valid position if blocked
            return; 
        }

        if (useBounds)
        {
            if (desiredPosition.x < minXZ.x || desiredPosition.x > maxXZ.x ||
                desiredPosition.z < minXZ.y || desiredPosition.z > maxXZ.y)
            {
                targetPosition = prevTargetPosition;
                return;
            }
        }

        prevTargetPosition = targetPosition;
        targetPosition = desiredPosition;
    }

    private void NormalizeRotation()
    {
        if (targetRotation.y >= 360f) targetRotation.y -= 360f;
        if (targetRotation.y < 0f) targetRotation.y += 360f;
    }
}