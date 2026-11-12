using UnityEngine;

public class InputTracker : MonoBehaviour
{
    public static InputTracker Instance { get; private set; }

    [Header("Stats for Current Scene")]
    [SerializeField] private int totalButtonPresses = 0;
    public int tapCount { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        totalButtonPresses = 0;
    }

    public void RegisterPress()
    {
        totalButtonPresses++;
        tapCount = totalButtonPresses;
        Debug.Log($"[Scene Tap Tracker] Current Taps: {totalButtonPresses}");
    }

    public int GetTotalPresses()
    {
        return totalButtonPresses;
    }
}