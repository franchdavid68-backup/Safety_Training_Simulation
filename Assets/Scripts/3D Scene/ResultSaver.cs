using Firebase.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ResultSaver : MonoBehaviour
{
    [Header("Script References")]
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private InputTracker tapTracker;
    public async void SaveSimulationResults()
    {
        string userId = FirebaseManager.Instance.user.UserId;
        string username = "Unknown user";
        string status = "";

        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

        try
        {
            //fetch user document to get the username
            DocumentSnapshot userDoc = await db.Collection("users").Document(userId).GetSnapshotAsync();
            if (userDoc.Exists && userDoc.ContainsField("username"))
            {
                username = userDoc.GetValue<string>("username");
            }

            //Extract from different scripts
            double duration = levelManager != null ? levelManager.EndTime : 0f;
            int errorCount = levelManager != null ? levelManager.currentErrors : 0;
            int tapCount = tapTracker != null ? tapTracker.tapCount : 0;

            Debug.Log($"Values : Duration: {duration}, Errors: {errorCount}, tapCount: {tapCount}");

            //  Assign an attempt status according to the amount of errors
            if (errorCount == 0)
            {
                status = "Passed";
            }
            if (errorCount > 0 && errorCount < 3)
            {
                status = "Needs Improvement";
            }
            if (errorCount == 3)
            {
                status = "Failed";
            }

            if (duration > 0 && tapCount > 0) // just a test to ensure that the wrong input is not saved
            {
                // Only test for best attempt if the simulation status has been passed
                if (status == "Passed")
                {
                    // Test for the leaderboard
                    await TestForBest(userId, duration, errorCount, tapCount, username, status);
                }

                //Firebase dictionary
                await FirestoreData.Instance.SaveSimulationResultAsync(userId, username, duration, tapCount, errorCount, status);
            }
            else
            {
                Debug.Log($"User: {username} could not save results");
            }


        }
        catch (System.Exception ex)
        {
            Debug.Log($"Could not fetch username from firestore: {ex.Message}");
        }

    }

    public async Task TestForBest(string userId, double duration, int errorCount, int tapCount, string username, string status)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

        try
        {
            //Point to leaderboard document 
            DocumentReference leaderRef = db.Collection("leaderboard").Document(userId);

            //Retreive the document
            DocumentSnapshot leadSnapshot = await leaderRef.GetSnapshotAsync();


            //Check if the document exists then extract and test
            if (leadSnapshot.Exists)
            {
                Dictionary<string, object> data = leadSnapshot.ToDictionary();
                //Tenary operator to safely extract data from firestore, testing if exists then extract else its 0
                double oldDuration = data.ContainsKey("duration") ? System.Convert.ToDouble(data["duration"]) : 0;
                int oldError = data.ContainsKey("errorCount") ? System.Convert.ToInt32(data["errorCount"]) : 0;
                int oldTap = data.ContainsKey("tapCount") ? System.Convert.ToInt32(data["tapCount"]) : 0;

                //Test if the duration, error count and tap counts are smaller or equal to the ones in the leaderboard
                if (duration <= oldDuration && oldError >= errorCount && oldTap >= tapCount && status == "Passed")
                {
                    //Save to the leaderboard in the FirestoreData manager

                    await FirestoreData.Instance.saveToLeaderboard(userId, username, duration, tapCount, errorCount);
                }
                else
                {
                    Debug.Log("Simulation does not beat the best attempt");
                }
            }
            else // Log their first attempt
            {
                await FirestoreData.Instance.saveToLeaderboard(userId, username, duration, tapCount, errorCount);
            }



        }
        catch (System.Exception ex)
        {
            Debug.Log($"Leaderboard test: {ex.Message}");
        }
    }


}
