using Firebase.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FirestoreData : MonoBehaviour
{
    public static FirestoreData Instance { get; private set; } // Singleton instance allows for global database access across scenes

    private FirebaseFirestore db; // reference to Cloud firestore database instance

    private void Awake()
    {
        //Ensure that only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        db = FirebaseFirestore.DefaultInstance; // initialize firestore reference
    }

    //Creates a new user document inside of the "users" collection in Firestore upon registration
    public async Task CreateUserDocumentAsync(string uid, string email, string username, string role, string empID)
    {
        //Check if database is ready
        if (db == null)
        {
            db = FirebaseFirestore.DefaultInstance;
        }
        DocumentReference userRef = db.Collection("users").Document(uid);

        //Package user details into a dictionary to store in Firestore
        Dictionary<string, object> userData = new Dictionary<string, object>
            {
                { "email", email },
                { "username", username },
                { "role", role },
                { "empID", empID },
                {"created_at", System.DateTime.Now }
            };

        //Write to the document asynchronously
        await userRef.SetAsync(userData);
        Debug.Log($"[FirestoreData] User document created for UID: {uid}");

    }

    //Fetch the users role from firestore document
    public async Task<string> GetUserRoleAsync(string uid)
    {
        if (db == null)
        {
            db = FirebaseFirestore.DefaultInstance;
        }

        try
        {
            DocumentReference userRef = db.Collection("users").Document(uid);
            DocumentSnapshot snapshot = await userRef.GetSnapshotAsync();

            //Check if the document exists and contails the role field
            if (snapshot.Exists && snapshot.ContainsField("role"))
            {
                return snapshot.GetValue<string>("role");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[FirestoreData] Error fetching user role for UID: {uid} - {ex.Message}");
        }

        return "Employee"; // Default fallback roll if an error or missing field occurs
    }

    //Fetch user document data into user model
    public async Task<UserModel> GetUserDocumentAsync(string uid)
    {
        if (db == null)
        {
            db = FirebaseFirestore.DefaultInstance;
        }
        try
        {
            DocumentReference userRef = db.Collection("users").Document(uid);
            DocumentSnapshot snapshot = await userRef.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                //Converts Firestore snapshop directly to the custom UserModel class
                UserModel userModel = snapshot.ConvertTo<UserModel>();
                return userModel;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[FirestoreData] Error fetching user document for UID: {uid} - {ex.Message}");
        }
        return null; // Return null if an error occurs
    }

    //Save simulation results to Firestore

    public async Task SaveSimulationResultAsync(string uid, string username, double duration, int tapCount, int errorCount, string status)
    {
        if (db == null)
        {
            db = FirebaseFirestore.DefaultInstance;
        }
        try
        {
            //If the document does not exist then firestore will automatically make a new one with the ID
            DocumentReference userRef = db.Collection("users").Document(uid);
            CollectionReference resultsRef = userRef.Collection("simulation_results");

            //Build the dictionery containing simulation metrics
            Dictionary<string, object> resultData = new Dictionary<string, object>
            {
                { "duration", duration },
                { "tapCount", tapCount },
                { "errorCount", errorCount },
                { "status", status },
                { "completedAt", System.DateTime.Now },
                {"username", username }
            };

            //Add the new result entry to the user's simulation_results subcollection
            await resultsRef.AddAsync(resultData);
            Debug.Log($"[FirestoreData] Simulation result saved for UID: {uid}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[FirestoreData] Error saving simulation result for UID: {uid} - {ex.Message}");
        }
    }

    //Save best attempts to leaderboard
    public async Task saveToLeaderboard(string lUid, string username, double duration, int tapCount, int errorCount)
    {
        if (db == null)
        {
            db = FirebaseFirestore.DefaultInstance;
        }
        try
        {
            //Make user document referece in the leaderbaord.
            DocumentReference userRef = db.Collection("leaderboard").Document(lUid);

            //Build the dictionary containing leaderboard metrics
            Dictionary<string, object> leaderData = new Dictionary<string, object>
            {
                {"duration", duration },
                {"errorCount", errorCount },
                {"tapCount", tapCount },
                {"username", username }
            };

            //Replace the users current best simulation with the best simulation
            await userRef.SetAsync(leaderData);
            Debug.Log("Leaderboard successfully updated");
        }
        catch (System.Exception ex)
        {
            Debug.Log($"Leaderboard error: {ex.Message}");
        }
    }

}