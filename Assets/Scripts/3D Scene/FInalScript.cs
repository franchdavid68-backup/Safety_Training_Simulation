using UnityEngine;

public class FinalScriptInteractable : MonoBehaviour, IInteractable
{
    [Header("Puzzle Requirements")]
    [SerializeField] private GameObject valvesTurnedObject;
    [SerializeField] private VentValveInteractable ventValve;
    [SerializeField] private Item requiredActuatorItem;

    [Header("Prompt Settings")]
    [SerializeField] private string actionPrompt = "Install New Actuator";
    [Header("Indicator Settings")]
    [SerializeField] private GameObject targetIndicator;

public void Awake(){
    
        if (targetIndicator != null)
            targetIndicator.SetActive(false);
}

    public void Interact()
    {
        // 1. Check if Valves are closed/turned
        bool areValvesTurned = (valvesTurnedObject != null && valvesTurnedObject.activeSelf);

        // 2. Check if Vent is open
        bool isVentOpen = (ventValve != null && ventValve.isVentOpen);

        // 3. Check if player has the new actuator in inventory
        bool hasNewActuatorInInventory = Inventory.Instance != null && Inventory.Instance.HasItem(requiredActuatorItem);


        if (!areValvesTurned)
        {
            string errorMessage = "Violation: Attempted to replace actuator while system supply valves were still open!";
            LevelManager.Instance.LogMistake(errorMessage);

            if (UINotificationManager.Instance != null)
            {
                UINotificationManager.Instance.ShowMessage(errorMessage);
            }
            else
            {
                Debug.LogError("UINotificationManager.Instance is NULL! Make sure it is in the scene."); // <-- Add this
            }
            return;
        }

        if (!isVentOpen)
        {
            string errorMessage = "Violation: Attempted to remove/replace actuator while system was still under pressure (Vent was closed)!";
            LevelManager.Instance.LogMistake(errorMessage);

            if (UINotificationManager.Instance != null)
            {
                UINotificationManager.Instance.ShowMessage(errorMessage);
            }
            return;
        }

        if (!hasNewActuatorInInventory)
        {
            string errorMessage = "Violation: You do not have the new actuator item in your inventory!";
            LevelManager.Instance.LogMistake(errorMessage);

            if (UINotificationManager.Instance != null)
            {
                UINotificationManager.Instance.ShowMessage(errorMessage);
            }
            return;
        }

        // --- WIN STATE CONDITION ---
        Debug.Log("Actuator successfully replaced under safe LOTO compliance!");
        LevelManager.Instance.CompleteLevel();

        ResultSaver saver = FindObjectOfType<ResultSaver>();
        if (saver != null)
        {
            saver.SaveSimulationResults();
        }
        else
        {
            Debug.LogWarning("Results were not able to be saved");
        }
    }

    public string GetDescription() => actionPrompt;
    public void Highlight() {

        if(targetIndicator != null){
            targetIndicator.SetActive(true);
        }
     }
    public void Unhighlight() {

                if(targetIndicator != null){
            targetIndicator.SetActive(false);
        }
     }
}