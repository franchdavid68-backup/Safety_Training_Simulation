using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmployeeCard : MonoBehaviour
{
    [Header("Employee Information")]
    [SerializeField] private TMP_Text employeeName;
    [SerializeField] private TMP_Text employeeId;
    [SerializeField] private TMP_Text employeePosition;
    [SerializeField] private TMP_Text errorCount;
    [SerializeField] private TMP_Text bestTime;
    private Action<string, string> onInspectCallback; // Holds instruction to switch panels 

    [Header("Buttons")]
    [SerializeField] private Button btnDelete;
    [SerializeField] private Button btnEdit;
    [SerializeField] private Button btnInspect;



    //Global variables to save information on each card
    private string userID; // Store the user ID for deletion or editing purposes
    public double BestTime { get; private set; }
    public string EmployeeName { get; private set; }
    public int Errors { get; private set; }


    public void SetEmployeeInfo(string uid, string name, string id, string position, int errors, float time, Action<string, string> inspectCallBack)
    {
        //extract the userID
        userID = uid;
        onInspectCallback = inspectCallBack;

        //Variable for BestTime
        BestTime = time;
        EmployeeName = name;
        Errors = errors;

        if (employeeName == null || employeeId == null || employeePosition == null || errorCount == null || bestTime == null)
        {
            Debug.LogError("One or more TMP_Text components are not assigned.");
            return;
        }
        employeeName.text = $"Name: {name}";
        employeeId.text = $"ID: {id}";
        employeePosition.text = $"Role: {position}";
        errorCount.text = errors.ToString();
        bestTime.text = time.ToString("F2");
        // Update the UI elements here if needed

        AdminPopUp adminPop = FindAnyObjectByType<AdminPopUp>();

        if (btnDelete != null)
        {

            btnDelete.onClick.RemoveAllListeners();
            btnDelete.onClick.AddListener(() => adminPop.ShowDeletePopup("Delete User", userID, this.gameObject)); // Call ShowDeletePopup with the stored userID and current card reference
        }

        if (btnInspect != null)
        {
            btnInspect.onClick.RemoveAllListeners();
            btnInspect.onClick.AddListener(() =>
            {
                // triggers the panel switch and data load in adminUI
                onInspectCallback?.Invoke(userID, name);
            });
        }
    }

    // Keep just in case the popup manager stops working
    /* private async void DeleteUser()
     {
         //test if the userID is null or empty
         if (string.IsNullOrEmpty(userID))
         {
             Debug.LogError("User ID is null or empty. Cannot delete user.");
             return;
         }

         Debug.Log($"Delete user with ID: {userID}");
         bool success = await DeleteUserFromFirebase(userID); // Call the DeleteUser method from FirebaseManager

         if (success)
         {
             //remove the card from the UI
             Destroy(gameObject);

         }

         deletePopup.SetActive(false); // Close the delete popup after deletion

     }

     private async Task<bool> DeleteUserFromFirebase(string uid) // Delete user from Firebase Firestore and Authentication
     {
         try
         {
             FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

             //first delete all simulations
             CollectionReference simuRef = db.Collection("users").Document(uid).Collection("simulation_results");
             QuerySnapshot simuSnapshot = await simuRef.GetSnapshotAsync(); // Get all simulation documents for the user

             //Delete each simulation document
             foreach (DocumentSnapshot doc in simuSnapshot.Documents)
             {
                 await doc.Reference.DeleteAsync(); // use delete async to delete each simulation document
             }

             //Then delete the user document
             await db.Collection("users").Document(uid).DeleteAsync(); // Delete the user document from Firestore
             Debug.Log($"Successfully deleted user with ID: {uid} and all associated simulation results.");
             return true;


         }
         catch (System.Exception ex)
         {
             Debug.LogError($"Failed to delete user with ID: {uid}. Error: {ex.Message}");
             return false;
         }
     }*/




}
