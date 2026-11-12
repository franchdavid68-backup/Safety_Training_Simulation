using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class UnlockableObject : MonoBehaviour, IInteractable
{
    private Renderer objRenderer;
    private MaterialPropertyBlock propBlock;
    
    private static readonly int EmissionColorID = Shader.PropertyToID("_EmissionColor");

    [Header("Glow Settings")]
    [ColorUsage(true, true)]
    [SerializeField] private Color glowColor = new Color(0.5f, 0.3f, 0.2f) * 2f;

    [Header("Unlock Requirements")]
    [SerializeField] private Item requiredItem;
    [SerializeField] private string lockedDescription = "Locked";
    [SerializeField] private string unlockedDescription = "Interact";

    [Header("Tutorial UI Popup (Optional)")]
    [SerializeField] private bool showTutorialOnInteract = true;
    [SerializeField] private string tutorialTitle;
    [TextArea(3, 5)]
    [SerializeField] private string tutorialDescription;
    [SerializeField] private Sprite tutorialImage;

    [Header("Events")]
    [Tooltip("Fires ONCE immediately when the player unlocks this object.")]
    [SerializeField] private UnityEvent onUnlocked;

    [Tooltip("Fires on subsequent interactions AFTER the object has already been unlocked.")]
    [SerializeField] private UnityEvent onInteractWhenUnlocked;

    [Header("State")]
    [SerializeField] private bool isUnlocked = false;

    private void Awake()
    {
        propBlock = new MaterialPropertyBlock();
        objRenderer = GetComponentInChildren<Renderer>();
    }

    public void Interact()
    {
        if (TutorialUIManager.Instance != null && TutorialUIManager.Instance.IsShowing)
            return;

        if (isUnlocked)
        {
            onInteractWhenUnlocked?.Invoke();
            TriggerTutorialUI(); 
            return;
        }

        bool canUnlock = requiredItem == null || 
                         (Inventory.Instance != null && Inventory.Instance.HasItem(requiredItem));

        if (canUnlock)
        {
            isUnlocked = true;
            Debug.Log($"[UnlockableObject] {gameObject.name} Unlocked!");

            onUnlocked?.Invoke();

            TriggerTutorialUI();
        }
        else
        {
            string itemName = requiredItem != null ? requiredItem.name : "Item";
            Debug.Log($"[UnlockableObject] {gameObject.name} is locked! Requires {itemName}.");
        }
    }

    private void TriggerTutorialUI()
    {
        if (showTutorialOnInteract && TutorialUIManager.Instance != null)
        {
            TutorialUIManager.Instance.ShowTutorial(tutorialTitle, tutorialDescription, tutorialImage);
        }
    }

    public string GetDescription()
    {
        if (isUnlocked) 
            return unlockedDescription;

        return requiredItem != null ? $"Requires {requiredItem.name}" : lockedDescription;
    }


    public void Highlight() 
    {
        if (objRenderer == null) return;

        propBlock ??= new MaterialPropertyBlock();

        objRenderer.GetPropertyBlock(propBlock);
        propBlock.SetColor(EmissionColorID, glowColor);
        objRenderer.SetPropertyBlock(propBlock);
    }

    public void Unhighlight() 
    {
        if (objRenderer == null) return;

        propBlock ??= new MaterialPropertyBlock();

        objRenderer.GetPropertyBlock(propBlock);
        propBlock.SetColor(EmissionColorID, Color.black);
        objRenderer.SetPropertyBlock(propBlock);
    }
}