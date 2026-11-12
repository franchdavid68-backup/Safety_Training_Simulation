using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance { get; private set; } // Singleton instance

    [Header("Firebase State")]
    [System.NonSerialized] public FirebaseAuth auth;
    [System.NonSerialized] public FirebaseUser user;
    public bool isInitialized = false; // flag to see if firebase is ready for use

    public void Awake()
    {
        // Ensure that only one instance exists across all scences
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
        //Check and resolve all necessary firebase dependencies before starting services
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
            }
        });
    }

    // Configures firebase authentication instances and event listeners once dependencies are ready
    private void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null); // trigger initial state check
        user = null;
        isInitialized = true;
        Debug.Log("[FirebaseManager] Initialized successfully.");
    }

    //Listenes for changes in the users authentication state whether signed in or signed out
    private void AuthStateChanged(object sender, EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null && auth.CurrentUser.IsValid();
            if (!signedIn && user != null)
            {
                Debug.Log($"Signed out: {user.UserId}");
            }

            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log($"Signed in: {user.UserId}");
            }
        }
    }

    // Clean up references when the object is destroyed
    private void OnDestroy()
    {
        if (auth != null)
        {
            auth.StateChanged -= AuthStateChanged;
            auth = null;
        }
    }

    // Authentication async tasks

    // Handles user sign-in using email and password
    public async Task<(bool success, string message)> SignInUserAsync(string email, string password)
    {
        //check if auth is initialized
        if (!isInitialized || auth == null)
        {
            Debug.LogWarning("[FirebaseManager] Firebase Auth is not yet ready. Retrying...");
            auth = FirebaseAuth.DefaultInstance; // Reinitialize auth if it was not ready

            if (auth == null)
            {
                return (false, "Failed to initialize Firebase Auth.");
            }
        }

        try
        {
            //await the firebase sign in task
            AuthResult result = await auth.SignInWithEmailAndPasswordAsync(email, password);
            user = result.User;
            return (true, "Login Successful");
        }
        catch (AggregateException ex)
        {
            // Parse exceptions thrown by tasks
            string errorMessage = ParseAuthError(ex);
            return (false, errorMessage);
        }
        catch (FirebaseException ex)
        {
            //Handle direct firebase exceptions
            string errorMessage = ParseAuthError(new AggregateException(ex));
            return (false, errorMessage);
        }
    }

    //Handle new user registration and triggers firestore docuemnt generation
    public async Task<(bool success, string message)> CreateUserAsync(string email, string password, string username, string empID, string role = "Employee")
    {
        //Admin login variables to login after completion of user create.
        string adminEmail = "admin@gmail.com";
        string adminPass = "Admin134";
        //Verify initialization state before executing registration
        if (!isInitialized || auth == null)
        {
            Debug.LogWarning("[FirebaseManager] Firebase Auth is not yet ready. Retrying...");
            auth = FirebaseAuth.DefaultInstance; // Reinitialize auth if it was not ready
            if (auth == null)
            {
                return (false, "Failed to initialize Firebase Auth.");
            }
        }

        try
        {
            //Create user account with email and password
            AuthResult result = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            FirebaseUser user = result.User;



            if (user != null)
            {
                //Set initial user profile details with display name
                UserProfile profile = new UserProfile
                {
                    DisplayName = username,
                    PhotoUrl = new Uri("https://placehold.co/400")
                };
                await user.UpdateUserProfileAsync(profile);

                //Call FirestoreData to create user document
                if (FirestoreData.Instance != null)
                {
                    await FirestoreData.Instance.CreateUserDocumentAsync(user.UserId, email, username, role, empID);
                }
                else
                {
                    Debug.LogError("[FirebaseManager] FirestoreData instance is null. Cannot create user document.");
                }
            }
            // Sign back in as the administrator
            await auth.SignInWithEmailAndPasswordAsync(adminEmail, adminPass);

            return (true, "Registration Successful");

        }
        catch (AggregateException ex)
        {
            string errorMessage = ParseAuthError(ex);
            return (false, errorMessage);

        }
        catch (FirebaseException ex)
        {
            string errorMessage = ParseAuthError(new AggregateException(ex));
            return (false, errorMessage);
        }
    }

    //Sign out the current user
    public void SignOut()
    {
        if (auth != null)
        {
            auth.SignOut();
            user = null;
        }
    }

    //Error handling

    //Flattens aggregationExceptions to extract specific Firebase error codes
    private string ParseAuthError(AggregateException ex)
    {
        foreach (Exception exception in ex.Flatten().InnerExceptions)
        {
            if (exception is FirebaseException firebaseEx)
            {
                var errorCode = (AuthError)firebaseEx.ErrorCode;
                return GetErrorMessage(errorCode);
            }
        }
        return "An unknown error occurred.";
    }

    // Maps Firebase AuthError codes to user-friendly messages
    private string GetErrorMessage(AuthError errorCode)
    {
        switch (errorCode)
        {
            case AuthError.InvalidEmail:
                return "Invalid email address.";
            case AuthError.WrongPassword:
                return "Incorrect password.";
            case AuthError.UserNotFound:
                return "User not found.";
            case AuthError.EmailAlreadyInUse:
                return "Email is already in use.";
            case AuthError.WeakPassword:
                return "Password is too weak.";
            default:
                return $"Authentication error: {errorCode}";
        }
    }

}