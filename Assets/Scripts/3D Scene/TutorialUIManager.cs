using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUIManager : MonoBehaviour
{
    public static TutorialUIManager Instance { get; private set; }

    [Header("UI Component References")]
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image displayImage;
    [SerializeField] private Button closeButton;

    public bool IsShowing { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (closeButton != null)
            closeButton.onClick.AddListener(HideTutorial);

        HideTutorial();
    }

    public void ShowTutorial(string title, string description, Sprite image = null)
{
    if (titleText != null) titleText.text = title;
    if (descriptionText != null) descriptionText.text = description;

    if (displayImage != null)
    {
        if (image != null)
        {
            displayImage.sprite = image;
            displayImage.gameObject.SetActive(true);
        }
        else
        {
            displayImage.gameObject.SetActive(false);
        }
    }

    if (popupPanel != null)
        popupPanel.SetActive(true);

    IsShowing = true;

    UIState.Push();
    
    Time.timeScale = 0f; 
}

public void HideTutorial()
{
    if (IsShowing)
    {
        UIState.Pop();
    }

    if (popupPanel != null)
        popupPanel.SetActive(false);

    IsShowing = false;

    Time.timeScale = 1f; 
}
}