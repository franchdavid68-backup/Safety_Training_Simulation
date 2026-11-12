using UnityEngine;

[RequireComponent(typeof(Collider))]
public class VentValveInteractable : MonoBehaviour, IInteractable
{
    [Header("Vent State")]
    public bool isVentOpen = false;

    [SerializeField] private string actionPromptOpen = "Turn off Vent";
    [SerializeField] private string actionPromptClosed = "Turn on Vent";

    public void Interact()
    {
        isVentOpen = !isVentOpen;

        if (isVentOpen)
        {
            Debug.Log("Vent opened. Pressure released.");
        }
        else
        {
            Debug.Log("Vent closed.");
        }
    }

    public string GetDescription()
    {
        return isVentOpen ? actionPromptOpen : actionPromptClosed;
    }

    public void Highlight() {  }
    public void Unhighlight() {}
}