using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    public bool IsDialogueOpen { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text promptText;
    [SerializeField] private Transform choicesButtonContainer;
    [SerializeField] private GameObject choiceButtonPrefab;

    [Header("UI Panels to Hide During Dialogue")]
    [Tooltip("Drag UI panels here that should hide while speaking (e.g. Crosshair, HUD, Interaction Text).")]
    [SerializeField] private GameObject gameplayUIPanel;
    [Tooltip("Drag your parent Mobile Controls UI panel here to hide the movement/interaction buttons during dialogue.")]
    [SerializeField] private GameObject mobileControlsPanel; 

    [Header("Player Control")]
    [Tooltip("Drag your combined First-Person Player script here.")]
    [SerializeField] private MonoBehaviour playerControllerScript;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    public void OpenDialogue(string prompt, List<DialogueChoice> choices)
    {
        IsDialogueOpen = true;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        if (gameplayUIPanel != null)
            gameplayUIPanel.SetActive(false);

        if (mobileControlsPanel != null)
            mobileControlsPanel.SetActive(false);

        if (playerControllerScript != null)
            playerControllerScript.enabled = false;

        if (promptText != null)
            promptText.text = prompt;

        UIState.Push();

        foreach (Transform child in choicesButtonContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var choice in choices)
        {
            GameObject buttonObj = Instantiate(choiceButtonPrefab, choicesButtonContainer);
            Button btn = buttonObj.GetComponent<Button>();
            TMP_Text btnText = buttonObj.GetComponentInChildren<TMP_Text>();

            string displayText = choice.choiceText;
            bool hasRequiredItem = true;

            if (choice.requiredItem != null)
            {
                if (Inventory.Instance != null)
                {
                    hasRequiredItem = Inventory.Instance.HasItem(choice.requiredItem);
                }

                if (!hasRequiredItem)
                {
                    displayText += $" (Requires: {choice.requiredItem.name})";
                }
            }

            if (btnText != null)
                btnText.text = displayText;

            if (btn != null)
            {
                if (hasRequiredItem)
                {
                    btn.interactable = true;
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(() => {
                        choice.onChoiceSelected?.Invoke();
                        CloseDialogue();
                    });
                }
                else
                {
                    btn.interactable = false;
                }
            }
        }
    }

    public void CloseDialogue()
    {
        IsDialogueOpen = false;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
        if (playerControllerScript != null)
            playerControllerScript.enabled = true;

        if (gameplayUIPanel != null)
            gameplayUIPanel.SetActive(true);

        if (mobileControlsPanel != null)
            mobileControlsPanel.SetActive(true);

        UIState.Pop();
    }
}