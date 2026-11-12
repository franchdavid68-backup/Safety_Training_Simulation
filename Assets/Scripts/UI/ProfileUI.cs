using Firebase.Firestore;
using System;
//using Unity.Android.Types;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text employeeName;

    [Header("Simulation Data")]
    [SerializeField] private TMP_Text bestTime;

    [SerializeField] private TMP_Text numAttempts;

    [Header("Navigation Manager")]
    public TestNavigationManager navigationManager;
    [SerializeField] private GameObject pnlSimulationDescription;

    [Header("Screen")]
    [SerializeField] private GameObject loginScreen;

    [Header("Buttons")]
    [SerializeField] private Button btnHome;
    [SerializeField] private Button btnLogout;
    [SerializeField] private Button btnSim;

    [Header("Sim Attemps UI")]

    [SerializeField] private Transform simulationAttempts; // content container
    [SerializeField] private GameObject simPrefab; //  the prefab

    //Globally declared simID variable beacuse it works this way
    private string simuationDoc;

    //On Enable, fetch the user data from firestore and display it on the UI
    private void OnEnable()
    {
        //load the user data from firestore and display it on the UI
        LoadUserData();

        //Load user simulation data from firestore and display it on the UI
        LoadSimulationData();
    }

    //start for the buttons
    public void Start()
    {
        if (btnHome != null)
        {
            btnHome.onClick.AddListener(() =>
            {
                navigationManager.GoBack();
            });
        }

        if (btnLogout != null)
        {

            btnLogout.onClick.AddListener(() =>
            {
                // Sign out the user
                FirebaseManager.Instance.SignOut();

                // Navigate back to the login screen
                navigationManager.OpenScreen(loginScreen);
            });

        }
        if (btnSim != null)
        {
            btnSim.onClick.AddListener(() =>
            {
                navigationManager.OpenScreen(pnlSimulationDescription);
            });
        }

    }
    public async void LoadUserData()
    {
        //Check if the user is logged in
        if (FirebaseManager.Instance == null || FirebaseManager.Instance.user == null)
        {
            Debug.LogError("[ProfileUI] No user is logged in.");
            return;
        }

        //Fetch the user ID from the authenticated user
        string uid = FirebaseManager.Instance.user.UserId;

        //Fetch the user document from Firestore
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        DocumentReference userRef = db.Collection("users").Document(uid);

        try
        {
            // Get the user document snapshot and check if it exists
            DocumentSnapshot snapshot = await userRef.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                string username = snapshot.GetValue<string>("username");

                // Update the UI with the fetched user data
                if (username != null)
                {
                    employeeName.text = $"Welcome, {username}!";
                }
                else
                {
                    employeeName.text = "Unknown User";
                }

            }
            else
            {
                Debug.LogError("[ProfileUI] User document does not exist.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ProfileUI] Error fetching user data: {ex.Message}");
        }
    }

    public async void LoadSimulationData()
    {
        string userId = FirebaseManager.Instance.user.UserId;
        //Test if firebase is connected
        if (FirebaseManager.Instance == null || FirebaseManager.Instance.user == null)
        {
            Debug.LogError("[ProfileUI] No user is logged in.");
            return;
        }

        string uid = FirebaseManager.Instance.user.UserId;
        Debug.Log($"[ProfileUI] Trying to fetch path: users/{uid}");

        // Fetch simulation data from Firestore
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        CollectionReference simReference = db.Collection("users").Document(uid).Collection("simulation_results");

        try
        {
            if (simulationAttempts != null)
            {
                //Destroy old children when reloading page
                foreach (Transform child in simulationAttempts)
                {
                    Destroy(child.gameObject);
                }
            }
            //Get all simulation snapshots for the user and sort by time
            QuerySnapshot allSimSnapshots = await simReference.OrderByDescending("completedAt").GetSnapshotAsync();
            int totalSimulations = allSimSnapshots.Count;

            //If there are no attempts then display "No attempts yet" in the UI
            if (totalSimulations == 0)
            {
                numAttempts.text = "No attempts yet";
                bestTime.text = "N/A";
                return;
                //add the notification panel
            }

            //Get the best time from the simulations if there are any attempts
            QuerySnapshot bestTimeSnapshot = await simReference.OrderBy("duration").Limit(1).GetSnapshotAsync();

            if (bestTimeSnapshot.Count > 0)
            {
                DocumentSnapshot bestSimDoc = bestTimeSnapshot.Documents.FirstOrDefault();
                float bestDuration = bestSimDoc.GetValue<float>("duration");
                bestTime.text = $"{bestDuration:F2} seconds";
                numAttempts.text = totalSimulations.ToString();
            }
            else
            {
                bestTime.text = "N/A";
            }

            // simulation card section
            int count = totalSimulations;
            foreach (DocumentSnapshot simDoc in allSimSnapshots.Documents)
            {
                simuationDoc = simDoc.Id;

                if (simDoc.Exists) //Check if the document exists
                {
                    //Extract the relevant fields from the simulation attempt document
                    float duration = simDoc.ContainsField("duration") ? simDoc.GetValue<float>("duration") : 0f;
                    int errors = simDoc.ContainsField("errorCount") ? simDoc.GetValue<int>("errorCount") : 0;
                    int taps = simDoc.ContainsField("tapCount") ? simDoc.GetValue<int>("tapCount") : 0;
                    string status = simDoc.ContainsField("status") ? simDoc.GetValue<string>("status") : "Unknown";

                    Timestamp completedAt = simDoc.ContainsField("completedAt") ? simDoc.GetValue<Timestamp>("completedAt") : Timestamp.FromDateTime(System.DateTime.UtcNow);

                    //Create a new simulation attempt card and set its data
                    GameObject attemptCard = Instantiate(simPrefab, simulationAttempts);
                    SimulationAttemptCard cardScript = attemptCard.GetComponent<SimulationAttemptCard>();

                    if (cardScript != null)
                    {
                        cardScript.SetAttemptData(count, errors, duration, taps, status, completedAt, userId, simuationDoc);
                    }

                }
                count--;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ProfileUI] Error fetching simulation data: {ex.Message}");

        }
    }
}
