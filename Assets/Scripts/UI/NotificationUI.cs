using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class NotificationUI : MonoBehaviour
{
    [Header("Notification UI")]
    public GameObject notificationPanel;
    public TMP_Text notificationTitleText;
    public TMP_Text notificationMessageText;
   

    public void ShowNotification(string title, string message)
    {
        if (notificationTitleText != null) notificationTitleText.text = title;
        if (notificationMessageText != null) notificationMessageText.text = message;
        if (notificationPanel != null) notificationPanel.SetActive(true);
    }

    public void CloseNotificationPanel()
    {
        if (notificationPanel != null) notificationPanel.SetActive(false);
    }



}
