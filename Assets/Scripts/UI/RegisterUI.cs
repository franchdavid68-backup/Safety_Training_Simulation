using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using Unity.VisualScripting;

public class RegisterUI : MonoBehaviour
{
    [Header("Input Fields")]
    [SerializeField] private TMP_InputField edtEmail;
    [SerializeField] private TMP_InputField edtPassword;
    [SerializeField] private TMP_InputField edtUsername;
    [SerializeField] private TMP_InputField edtEmpId;

    [SerializeField] private Button btnRegister;
    [SerializeField] private Button btnBack;

    [Header("FeedBack")]
    [SerializeField] private TMP_Text statusText;

    [Header("Navigation Reference")]
    public TestNavigationManager navigationManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (btnRegister != null)
        {
            btnRegister.onClick.AddListener(RegisterUser);
        }

        if (btnBack != null)
        {
            btnBack.onClick.AddListener(GoBack);
        }
    }

    private async void RegisterUser()
    {
        // extract from input fields
        string email = edtEmail.text;
        string password = edtPassword.text;
        string username = edtUsername.text;
        string empID = edtEmpId.text;
        string role = "Employee";

        //test if all fields are filled

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(username))
        {
            SetStatus("[RegisterUI] Please fill in all fields.", Color.yellow);
            return;
        }
        btnRegister.interactable = false;
        SetStatus("Creating account...", Color.white);

        var (success, message) = await FirebaseManager.Instance.CreateUserAsync(email, password, username, empID, role);
 
        btnRegister.interactable = true;
        if (success)
        {
            SetStatus($"[RegisterUI] User registered successfully: {FirebaseManager.Instance.user.Email}", Color.green);

            //Clear the input fields after successful registration
            edtEmail.text = "";
            edtPassword.text = "";
            edtUsername.text = "";
            edtEmpId.text = "";

        }
        else
        {
            SetStatus($"[RegisterUI] Registration failed: {message}", Color.red);
        }

        btnRegister.interactable = true;
    }

    private void GoBack()
    {
        if (navigationManager != null)
        {
            navigationManager.GoBack();
        }
    }

    private void SetStatus(string message, Color color)
    {
        if (statusText != null)
        {
            statusText.text = message;
            statusText.color = color;
        }
    }

}
