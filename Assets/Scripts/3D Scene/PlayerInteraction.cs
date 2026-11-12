using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class PlayerInteraction : MonoBehaviour 
{
    [Header("Settings")]
    public Camera mainCam;
    public float interactionDistance = 2f;
    
    [Header("BoxCast Settings")]
    [Tooltip("Width, Height, and Depth of the interaction box extending from the camera.")]
    [SerializeField] private Vector3 boxSize = new Vector3(0.6f, 0.4f, 0.4f);
    public LayerMask interactableLayer = ~0;

    [Header("UI References")]
    public GameObject interactionUI;
    public TextMeshProUGUI interactionText;

    [Header("Player References")]
    public Inventory inventory;

    [Header("Mobile Touch Buttons")]
    [SerializeField] private TouchMovementButton cycleButton;
    [SerializeField] private TouchMovementButton interactButton;

    private List<IInteractable> detectedInteractables = new List<IInteractable>();
    private int selectedIndex = 0;
    private IInteractable currentInteractable;

    private void Awake()
    {
        if (mainCam == null)
            mainCam = Camera.main;

        if (inventory == null)
            inventory = GetComponent<Inventory>();
    }

    private void Update() 
    {
        if (TutorialUIManager.Instance != null && TutorialUIManager.Instance.IsShowing)
        {
            ClearSelection();
            if (interactionUI != null && interactionUI.activeSelf) interactionUI.SetActive(false);
            return;
        }

        ScanForInteractables();
        HandleMobileCycling();
        HandleMobileInteractionInput();
        UpdateUIAndHighlights();
    }

    private void ScanForInteractables() 
    {
        if (mainCam == null) return;

        detectedInteractables.Clear();

        Vector3 boxCenter = mainCam.transform.position;
        Vector3 halfExtents = boxSize * 0.5f;
        Quaternion orientation = mainCam.transform.rotation;
        
        RaycastHit[] hits = Physics.BoxCastAll(boxCenter, halfExtents, mainCam.transform.forward, orientation, interactionDistance, interactableLayer);

        HashSet<IInteractable> uniqueInteractables = new HashSet<IInteractable>();

        foreach (var hit in hits)
        {
            IInteractable interactable = null;

            if (hit.collider.TryGetComponent<IInteractable>(out interactable) ||
                (interactable = hit.collider.GetComponentInParent<IInteractable>()) != null ||
                (interactable = hit.collider.GetComponentInChildren<IInteractable>()) != null)
            {
                if (interactable != null)
                {
                    uniqueInteractables.Add(interactable);
                }
            }
        }

        detectedInteractables.AddRange(uniqueInteractables);

        if (detectedInteractables.Count > 0)
        {
            selectedIndex = Mathf.Clamp(selectedIndex, 0, detectedInteractables.Count - 1);
        }
        else
        {
            selectedIndex = 0;
        }
    }

    private void HandleMobileCycling()
    {
        if (detectedInteractables.Count <= 1) return;

        if (cycleButton != null && cycleButton.WasPressedThisFrame())
        {
            selectedIndex = (selectedIndex + 1) % detectedInteractables.Count;
        }
    }

    private void HandleMobileInteractionInput()
    {
        if (detectedInteractables.Count == 0) return;

        if (interactButton != null && interactButton.WasPressedThisFrame()) 
        {
            currentInteractable?.Interact();
        }
    }

    private void UpdateUIAndHighlights()
    {
        IInteractable targetInteractable = detectedInteractables.Count > 0 ? detectedInteractables[selectedIndex] : null;

        if (targetInteractable != currentInteractable)
        {
            if (currentInteractable != null)
            {
                currentInteractable.Unhighlight();
            }

            currentInteractable = targetInteractable;

            if (currentInteractable != null)
            {
                currentInteractable.Highlight();
            }
        }

        if (currentInteractable != null)
        {
            if (interactionUI != null && !interactionUI.activeSelf)
            {
                interactionUI.SetActive(true);
            }

            if (interactionText != null)
            {
                string counterHint = detectedInteractables.Count > 1 ? $" ({selectedIndex + 1}/{detectedInteractables.Count} - Tap Cycle to switch)" : "";
                interactionText.text = $"Tap Interact to {currentInteractable.GetDescription()}{counterHint}";
            }
        }
        else
        {
            if (interactionUI != null && interactionUI.activeSelf)
            {
                interactionUI.SetActive(false);
            }
        }
    }

    private void ClearSelection()
    {
        if (currentInteractable != null)
        {
            currentInteractable.Unhighlight();
            currentInteractable = null;
        }
        detectedInteractables.Clear();
    }

    private void OnDisable()
    {
        ClearSelection();
    }

    
}