using System.Collections.Generic;
using UnityEngine;

public class DialogueInteractable : MonoBehaviour, IInteractable
{
    private Renderer objRenderer;
    private MaterialPropertyBlock propBlock;
    private static readonly int EmissionColorID = Shader.PropertyToID("_EmissionColor");
    
    [Header("Prompt Settings")]
    [SerializeField] private string actionPrompt = "Inspect";

    [Header("Glow Settings")]
    [ColorUsage(true, true)] 
    [SerializeField] private Color glowColor = new Color(0f, 0.8f, 0.8f) * 2f; 

    [Header("Indicator Settings")]
    [SerializeField] private GameObject targetIndicator;

    [Header("Visual Novel Prompt")]
    [SerializeField] [TextArea] private string dialoguePrompt = "How would you like to handle this safety hazard?";
    [SerializeField] private List<DialogueChoice> choices = new List<DialogueChoice>();

    private void Awake()
    {
        objRenderer = GetComponent<Renderer>();
        if (objRenderer == null)
        {
            objRenderer = GetComponentInChildren<Renderer>();
        }
                if (targetIndicator != null)
            targetIndicator.SetActive(false);
    }

    public void Interact()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OpenDialogue(dialoguePrompt, choices);
        }
        else
        {
            Debug.LogWarning("DialogueManager instance not found in scene!");
        }
    }

    public string GetDescription() => actionPrompt;

    public void Highlight() 
    {
        if (targetIndicator != null)
        {
            targetIndicator.SetActive(true);
        }
    }

    public void Unhighlight() 
    {
        if (targetIndicator != null)
        {
            targetIndicator.SetActive(false);
        }
    }
}