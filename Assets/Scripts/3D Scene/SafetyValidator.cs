using UnityEngine;

public class SafetyValidator : MonoBehaviour
{
    public static SafetyValidator Instance { get; private set; }

    [Header("Level State References")]
    [SerializeField] private VentValveInteractable ventValve;       
    [SerializeField] private GameObject valvesTurnedObject;         

    [Header("Actuator Stage Models")]
    [SerializeField] private GameObject baseActuator;   
    [SerializeField] private GameObject actuatorMissingTop;  
    [SerializeField] private GameObject fakeActuatorEmpty;    
    [SerializeField] private GameObject fixedActuatorModel;  

    [Header("Audio (Optional)")]
    [SerializeField] private AudioClip errorSound;
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }


    public void TryRemoveActuatorTop()
    {
        if (!AreConditionsSafe()) return; 

        Debug.Log("Top removed safely.");
        if (baseActuator != null) baseActuator.SetActive(false);
        if (actuatorMissingTop != null) actuatorMissingTop.SetActive(true);
    }


    public void TryRemoveBasePlate()
    {
        if (!AreConditionsSafe()) return; 

        Debug.Log("Baseplate removed safely.");
        if (actuatorMissingTop != null) actuatorMissingTop.SetActive(false);
        if (fakeActuatorEmpty != null) fakeActuatorEmpty.SetActive(true);
    }


    public void TryInstallNewActuator()
    {
        if (!AreConditionsSafe()) return;

        Debug.Log("New actuator installed successfully!");
        if (fakeActuatorEmpty != null) fakeActuatorEmpty.SetActive(false);
        if (fixedActuatorModel != null) fixedActuatorModel.SetActive(true);

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.CompleteLevel();
        }
    }


    private bool AreConditionsSafe()
    {
        bool areValvesClosed = (valvesTurnedObject != null && valvesTurnedObject.activeSelf);
        if (!areValvesClosed)
        {
            LogSafetyViolation("Safety Violation: Action attempted while block valves were still open!");
            return false;
        }

        bool isVentOpen = (ventValve != null && ventValve.isVentOpen);
        if (!isVentOpen)
        {
            LogSafetyViolation("Safety Hazard: Action attempted while system was still under pressure (Vent closed).");
            return false;
        }

        return true;
    }

    private void LogSafetyViolation(string errorMessage)
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.LogMistake(errorMessage);
        }
        
        if (errorSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(errorSound);
        }
        
        Debug.LogWarning($"[ACTION BLOCKED]: {errorMessage}");
    }
}