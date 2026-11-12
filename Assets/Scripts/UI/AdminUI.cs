using Firebase.Firestore;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AdminUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button btnRegister;
    [SerializeField] private Button btnLogout;
    [SerializeField] private Button btnInspect;

    [Header("Navigation Manger")]
    public TestNavigationManager navigationManager;
    public NotificationUI notificationManager;

    [Header("Panel References")]
    [SerializeField] public GameObject loginScreen;
    [SerializeField] public GameObject adminScreen;
    [SerializeField] public GameObject registerScreen;
    [SerializeField] public GameObject inspectEmployeeScreen;
    [SerializeField] public InspectEmployeeUI employeeUI;

    [Header("Scroll View components")]

    [SerializeField] private Transform contentContainer; // Scroll View Content Container
    [SerializeField] private GameObject employeeCardPrefab; // Prefab for Employee Card

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (btnRegister != null)
        {
            btnRegister.onClick.AddListener(navRegister);
        }

        if (btnLogout != null)
        {
            btnLogout.onClick.AddListener(navLogout);
        }
        if (btnInspect != null)
        {
            btnInspect.onClick.AddListener(navInspect);
        }
    }

    //On Enable, fetch the employee data from firestore and display it on the UI
    private void OnEnable()
    {
        //Load employee data from firestore and display it on the UI
        LoadAllEmployeeData();
    }

    private void navInspect()
    {
        navigationManager.OpenScreen(inspectEmployeeScreen);
    }

    private void navRegister()
    {
        navigationManager.OpenScreen(registerScreen);
    }

    private void navLogout()
    {
        FirebaseManager.Instance.SignOut();
        navigationManager.OpenScreen(loginScreen);
    }

    // Load all employee data from Firestore and display it in the scroll view
    private async void LoadAllEmployeeData()
    {
        //Check if the user is logged in
        if (FirebaseManager.Instance == null || FirebaseManager.Instance.user == null)
        {
            Debug.LogError("[AdminUI] No user is logged in.");
            return;
        }


        // Clear any previous employee cards in the scroll view
        foreach (Transform child in contentContainer)
        {
            if (child != null)
            {
                Destroy(child.gameObject);
            }
        }

        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

        try
        {
            //Fetch all documents from the top-level "users" collection
            QuerySnapshot usersSnapshot = await db.Collection("users").GetSnapshotAsync();

            if (usersSnapshot.Count == 0)
            {
                notificationManager.ShowNotification("Error", "No Users Found");
            }
            else
            {
                //Loop through each user document and fetch the best simulation result from the "simulation_results" subcollection
                foreach (DocumentSnapshot document in usersSnapshot.Documents)
                {
                    string role = document.ContainsField("role") ? document.GetValue<string>("role") : "Unassigned Role";

                    if (role == "Administrator")
                    {
                        //Skip admin users
                        continue;
                    }

                    string username = document.ContainsField("username") ? document.GetValue<string>("username") : "Unknown User";
                    string empID = document.ContainsField("empID") ? document.GetValue<string>("empID") : "Unknown ID";
                    string userId = document.Id; // Get the document ID as userId


                    float duration = 0;
                    int errors = 0;
                    int taps = 0;

                    //Query the "employees" subcollection for the current user document
                    QuerySnapshot bestSimSnapshot = await db.Collection("users").Document(userId).Collection("simulation_results").OrderBy("duration").Limit(1).GetSnapshotAsync();

                    //Check if there are any simulation results
                    if (bestSimSnapshot.Count > 0)
                    {
                        DocumentSnapshot bestSimDocument = bestSimSnapshot.Documents.FirstOrDefault();
                        if (bestSimDocument != null || bestSimDocument.Exists)
                        {
                            duration = bestSimDocument.GetValue<float>("duration");
                            errors = bestSimDocument.GetValue<int>("errorCount");
                            taps = bestSimDocument.GetValue<int>("tapCount");
                        }
                    }

                    //Instantiate the employee card prefab and set its parent to the content container
                    GameObject employeeCard = Instantiate(employeeCardPrefab, contentContainer);

                    //popilate the employee card with the fetched data
                    EmployeeCard cardScript = employeeCard.GetComponent<EmployeeCard>();
                    if (cardScript != null)
                    {
                        cardScript.SetEmployeeInfo(userId, username, empID, role, errors, duration, (clickedId, clickedUsername) =>
                        {
                            //Hide admin screen
                            if (adminScreen != null)
                            {
                                navigationManager.OpenScreen(adminScreen);
                            }

                            //show profile page
                            if (inspectEmployeeScreen != null)
                            {
                                navigationManager.OpenScreen(inspectEmployeeScreen);
                            }

                            if (employeeUI != null)
                            {
                                employeeUI.LoadUserSimulationAttempts(clickedId, clickedUsername);
                            }
                        });
                    }
                    else
                    {
                        Debug.Log($"Error: [AdminUI] EmployeeCard script not found on the prefab.");
                        notificationManager.ShowNotification("Error: [AdminUI]", "EmployeeCard script not found on the prefab.");
                    }
                }
            }
        }
        catch (System.Exception ex)
        {

            notificationManager.ShowNotification("Error: [AdminUI]", $"Error fetching data: {ex.Message}");
        }

    }
}
