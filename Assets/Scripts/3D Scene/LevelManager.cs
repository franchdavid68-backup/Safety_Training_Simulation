using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Error & Loss Settings")]
    [SerializeField] private int maxErrors = 3;
    public int currentErrors { get; private set; } = 0;
    public double EndTime { get; private set; }

    private float startTime;
    private float finalTime;
    private bool isGameEnded = false;

    [Header("UI Panels")]
    [SerializeField] private GameObject winScreenPanel;
    [SerializeField] private GameObject gameOverScreenPanel;

    [Header("UI Text Displays (Optional)")]
    [SerializeField] private TMP_Text winStatsText;
    [SerializeField] private TMP_Text gameOverStatsText;

    [Header("HUD Timer Display")]
    [SerializeField] private TMP_Text timerText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        startTime = Time.time;
        currentErrors = 0;
        isGameEnded = false;

        if (winScreenPanel != null) winScreenPanel.SetActive(false);
        if (gameOverScreenPanel != null) gameOverScreenPanel.SetActive(false);
    }

    private void Update()
    {
        if (!isGameEnded && timerText != null)
        {
            float elapsedTime = Time.time - startTime;
            
            int minutes = Mathf.FloorToInt(elapsedTime / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);
            timerText.text = $"{minutes:00}:{seconds:00}";
            

        }
    }

    public void LogMistake(string errorDescription)
    {
        if (isGameEnded) return;

        currentErrors++;
        Debug.LogWarning($"Safety Mistake Logged: {errorDescription}. Errors: {currentErrors}/{maxErrors}");

        if (UINotificationManager.Instance != null)
        {
            UINotificationManager.Instance.ShowMessage(errorDescription);
        }

        if (currentErrors >= maxErrors)
        {
            TriggerGameOver(errorDescription);

            //Save results
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
    }

    //Level completed script
    public void CompleteLevel()
    {
        if (isGameEnded) return;
        isGameEnded = true;

        double finalTime = Time.time - startTime;
        EndTime = finalTime;
        Debug.Log($"Level Complete! Time: {finalTime:F2} seconds. Errors: {currentErrors}");

        if (winScreenPanel != null)
            winScreenPanel.SetActive(true);

        if (winStatsText != null)
        {
            winStatsText.text = $"Completion Time: {finalTime:F1}s\nErrors Made: {currentErrors} / {maxErrors}";
        }

        UIState.Push();
        Time.timeScale = 0f;
    }

    //Game over script
    private void TriggerGameOver(string lastErrorReason)
    {
        if (isGameEnded) return;
        isGameEnded = true;

        double finalTime = Time.time - startTime;
        EndTime = finalTime;

        Debug.Log("Game Over! Maximum safety errors reached.");

        if (gameOverScreenPanel != null)
            gameOverScreenPanel.SetActive(true);

        if (gameOverStatsText != null)
        {
            gameOverStatsText.text = $"Failure Reason: {lastErrorReason}\nTotal Errors: {currentErrors} / {maxErrors}\nTime Elapsed: {finalTime:F1}s";
        }

        UIState.Push();
        Time.timeScale = 0f;
    }

    public void RestartCurrentScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadSceneByName(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}