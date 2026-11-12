using Firebase.Firestore;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdminPopUp : MonoBehaviour
{
    [Header("Delete Popup")]
    [SerializeField] private GameObject pnlDelete;
    [SerializeField] private TMP_Text txtHeading;
    [SerializeField] private Button btnYes;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private string userID; // Store the user ID for deletion or editing purposes
    private string simulationID; // Store the simulation ID for deletion or editing purposes
    private GameObject recordCard; // Store the reference to the record card for deletion
    void Start()
    {
        pnlDelete.SetActive(false); // Ensure the delete popup is hidden initially

        if (btnYes != null)
        {
            if (string.IsNullOrEmpty(simulationID))
            {
                btnYes.onClick.AddListener(() => ConfirmDeleteUser()); // Call ConfirmDeleteUser with the stored userID

            }


        }
    }

    public void ShowDeletePopup(string heading, string uid, GameObject currentCard)
    {
        txtHeading.text = heading; // Set the heading text
        pnlDelete.SetActive(true); // Show the delete popup
        userID = uid; // Store the user ID for deletion
        recordCard = currentCard; // Store the reference to the record card for deletion
    }



    //Confirm deletion of user and all associated simulation results
    private async void ConfirmDeleteUser()
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
            Destroy(recordCard);

        }
        userID = null; // Clear the user ID
        simulationID = null; // Clear the simulation ID
        pnlDelete.SetActive(false); // Close the delete popup after deletion
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
    }



    public void CancelDelete()
    {
        pnlDelete.SetActive(false); // Hide the delete popup
        userID = null; // Clear the user ID
        simulationID = null; // Clear the simulation ID
    }


}
