using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class UINotificationManager : MonoBehaviour
{
    public static UINotificationManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private Transform notificationContainer; 
    [SerializeField] private GameObject notificationPrefab; 

    [Header("Settings")]
    [SerializeField] private int maxStackedMessages = 3;  
    [SerializeField] private float displayDuration = 3f;

    private readonly List<GameObject> activeNotifications = new List<GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ShowMessage(string message)
    {
        if (notificationContainer == null || notificationPrefab == null)
        {
            Debug.LogError("[UINotificationManager] Container or Prefab reference is missing in the Inspector!");
            return;
        }

        if (activeNotifications.Count >= maxStackedMessages)
        {
            GameObject oldest = activeNotifications[0];
            activeNotifications.RemoveAt(0);
            Destroy(oldest);
        }

        GameObject newNotification = Instantiate(notificationPrefab, notificationContainer);
        
        TMP_Text textComponent = newNotification.GetComponentInChildren<TMP_Text>();
        if (textComponent != null)
        {
            textComponent.text = message;
        }

        activeNotifications.Add(newNotification);

        StartCoroutine(RemoveNotificationRoutine(newNotification, displayDuration));
    }

    private IEnumerator RemoveNotificationRoutine(GameObject notificationObj, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (notificationObj != null)
        {
            activeNotifications.Remove(notificationObj);
            Destroy(notificationObj);
        }
    }
}