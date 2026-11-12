using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class LoginUI : MonoBehaviour
{
    [Header("Input Fields")]
    public TMP_InputField loginEmail;
    public TMP_InputField loginPassword;

    [Header("Buttons & Feedback")]
    public Button loginButton;
    public GameObject notificationPanel;
    public TMP_Text notificationTitleText;
    public TMP_Text notificationMessageText;

    [Header("Navigation Reference")]
    public TestNavigationManager navigationManager;
    public NotificationUI notificationManager;

    [Header("Panel References")]
    public GameObject homePanel;
    public GameObject adminPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //If login is rememebered, skip login
        if(FirebaseManager.Instance != null && FirebaseManager.Instance.auth != null)
        {
            if (FirebaseManager.Instance.auth.CurrentUser != null)
            {
                Debug.Log("User already logged in. Skipping to home page");
                if (navigationManager != null)
                {
                    navigationManager.OpenScreen(homePanel);
                }
                
            }
        }
      
        if (loginButton != null)
        {
            loginButton.onClick.AddListener(OnLoginClicked);
        }
    }

    public async void OnLoginClicked()
    {
        string email = loginEmail.text;
        string password = loginPassword.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            notificationManager.ShowNotification("Error", "Please fill in all fields.");
            return;
        }

        loginButton.interactable = false;

        //call the login function from FirebaseManager
        var(success, message) = await FirebaseManager.Instance.SignInUserAsync(email, password);

        loginButton.interactable = true;

        if (success)
        {
            //Reset the input fields
            loginEmail.text = "";
            loginPassword.text = "";

            //get the uid
            string uid = FirebaseManager.Instance.user.UserId;

            //fetch role from FirestoreData.cs
            string role = await FirestoreData.Instance.GetUserRoleAsync(uid);

            //Route the user to the appropriate panel based on their role
            if (role == "Administrator")
            {
                notificationManager.ShowNotification("Login Successful", "Welcome Admin!");
                // Navigate to the admin panel or perform other actions for admin users
                if (navigationManager != null)
                {
                    navigationManager.OpenScreen(adminPanel); // Replace with your admin panel reference
                }
            }
            else if (role == "Employee")
            {
                notificationManager.ShowNotification("Login Successful", "Welcome User!");
                // Navigate to the user panel or perform other actions for regular users
                if (navigationManager != null)
                {
                    navigationManager.OpenScreen(homePanel); // Replace with your user panel reference
                }
            }
           
            

        }
        else
        {
            notificationManager.ShowNotification("Login Failed", message);
        }
    }

   /* public void ShowNotification(string title, string message)
    {
        if(notificationTitleText != null) notificationTitleText.text = title;
        if (notificationMessageText != null) notificationMessageText.text = message;
        if (notificationPanel != null) notificationPanel.SetActive(true);
    }

    public void CloseNotificationPanel()
    {
        if (notificationPanel != null) notificationPanel.SetActive(false);
    }*/

}
