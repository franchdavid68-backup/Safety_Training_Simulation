using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DroppedItem : MonoBehaviour, IInteractable
{
    private Renderer objRenderer;
    private MaterialPropertyBlock propBlock;
    private static readonly int EmissionColorID = Shader.PropertyToID("_EmissionColor");

    [Header("Glow Settings")]
    [ColorUsage(true, true)]
    [SerializeField] private Color glowColor = new Color(0.5f, 0.3f, 0.2f) * 2f; 

    [Header("Item Data")]
    public Item item;
    [Header("References")]
    [SerializeField] private Transform visualHolder; // Assign your VisualHolder object here
    [SerializeField] private GameObject targetIndicator;

    [Header("State")]
    public bool pickedUp = false;

    private void Awake()
    {
        propBlock = new MaterialPropertyBlock();
        if (targetIndicator != null)
            targetIndicator.SetActive(false);
    }

    private void Start()
    {
        if (item != null)
        {
            Initialize(item);
        }
    }

public void Initialize(Item item)
{
    this.item = item;

    if (visualHolder != null)
    {
        foreach (Transform child in visualHolder)
        {
            Destroy(child.gameObject);
        }

        if (item != null && item.prefab != null)
        {
            GameObject visual = Instantiate(item.prefab, visualHolder, false);
            
            // reset local position, rotation, and scale just to be safe
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localRotation = Quaternion.identity;
            visual.transform.localScale = Vector3.one; // Forces scale back to normal (1,1,1)

            objRenderer = visual.GetComponentInChildren<Renderer>();
        }
    }
}

    // --- IInteractable Implementation ---

public void Interact()
{
    if (pickedUp || item == null) return;

    if (Inventory.Instance != null)
    {
        pickedUp = true;
        Inventory.Instance.AddItem(item);
        Destroy(gameObject);
    }
    else
    {
        Debug.LogWarning("[DroppedItem] No Inventory instance found in the scene!");
    }
}

    public string GetDescription()
    {
        return item != null ? $"Pick up {item.name}" : "Pick up Item";
    }

public void Highlight() 
{
    if (objRenderer == null) return;

    propBlock ??= new MaterialPropertyBlock();

    objRenderer.GetPropertyBlock(propBlock);
    propBlock.SetColor(EmissionColorID, glowColor);
    objRenderer.SetPropertyBlock(propBlock);

        if (targetIndicator != null)
        {
            targetIndicator.SetActive(true);
        }

}

public void Unhighlight() 
{
    if (objRenderer == null) return;

    propBlock ??= new MaterialPropertyBlock();

    objRenderer.GetPropertyBlock(propBlock);
    propBlock.SetColor(EmissionColorID, Color.black); 
    objRenderer.SetPropertyBlock(propBlock);

            if (targetIndicator != null)
        {
            targetIndicator.SetActive(false);
        }
}
}