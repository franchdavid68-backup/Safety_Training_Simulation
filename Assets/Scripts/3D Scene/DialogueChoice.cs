using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct DialogueChoice
{
    [Tooltip("The text displayed on the choice button.")]
    public string choiceText;

    [Tooltip("Leave empty if no item is required. If assigned, option is greyed out unless the player has it.")]
    public Item requiredItem;

    [Tooltip("Events triggered when this choice is successfully selected.")]
    public UnityEvent onChoiceSelected;
}