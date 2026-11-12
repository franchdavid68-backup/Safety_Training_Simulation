using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TutorialInteractable : MonoBehaviour, IInteractable
{
    [Header("Prompt Settings")]
    [SerializeField] private string actionPrompt = "Read Notice";

    [Header("Tutorial Content")]
    [SerializeField] private string title = "Tutorial Title";
    
    [TextArea(3, 8)]
    [SerializeField] private string description = "Enter tutorial text here...";
    
    [SerializeField] private Sprite tutorialImage;

    [Header("Highlight Settings")]
    [SerializeField] private Renderer objectRenderer;
    [SerializeField] private Color highlightColor = Color.yellow;
    
    [Header("Indicator Settings")]
    [SerializeField] private GameObject targetIndicator;

    private MaterialPropertyBlock propBlock;
    private static readonly int EmissionColorID = Shader.PropertyToID("_EmissionColor");

    private void Awake()
    {
        if (objectRenderer == null)
            objectRenderer = GetComponentInChildren<Renderer>();

        propBlock = new MaterialPropertyBlock();

        if (targetIndicator != null)
            targetIndicator.SetActive(false);
    }

    public void Interact()
    {
        if (TutorialUIManager.Instance != null)
        {
            TutorialUIManager.Instance.ShowTutorial(title, description, tutorialImage);
        }
        else
        {
            Debug.LogWarning("TutorialUIManager instance is missing from the scene!");
        }
    }

    public string GetDescription() => actionPrompt;

    public void Highlight()
    {
        if (objectRenderer != null)
        {
            objectRenderer.GetPropertyBlock(propBlock);
            propBlock.SetColor(EmissionColorID, highlightColor);
            objectRenderer.SetPropertyBlock(propBlock);
        }

        if (targetIndicator != null)
        {
            targetIndicator.SetActive(true);
        }
    }

    public void Unhighlight()
    {
        if (objectRenderer != null)
        {
            objectRenderer.GetPropertyBlock(propBlock);
            propBlock.SetColor(EmissionColorID, Color.black);
            objectRenderer.SetPropertyBlock(propBlock);
        }

        if (targetIndicator != null)
        {
            targetIndicator.SetActive(false);
        }
    }
}