using System.Collections.Generic;
using UnityEngine;

public class TestNavigationManager : MonoBehaviour
{


    //Assign screen name to the corresponding GameObject in the inspector
    [Header("UI Screens")]
    [SerializeField] private GameObject loginScreen;
    [SerializeField] private GameObject homeScreen;
    [SerializeField] private GameObject profileScreen;
    [SerializeField] private GameObject descriptionScreen;
    [SerializeField] public GameObject adminScreen;
    [SerializeField] private GameObject registrationScreen;
    [SerializeField] private GameObject editEmployeeScreen;

    //Track navigation history
    private Stack<GameObject> screenHistory = new Stack<GameObject>();
    private GameObject currentScreen;



    private void Awake()
    {
        InitializeScreen();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void InitializeScreen()
    {
        loginScreen.SetActive(true);
        homeScreen.SetActive(false);
        profileScreen.SetActive(false);
        descriptionScreen.SetActive(false);
        adminScreen.SetActive(false);
        registrationScreen.SetActive(false);
        editEmployeeScreen.SetActive(false);

        //establish initial screen
        currentScreen = loginScreen;
        screenHistory.Push(currentScreen);

    }

    public void OpenScreen(GameObject targetScreen)
    {
        if (targetScreen == null || targetScreen == currentScreen)
        {
            return;
        }

        //hide current screen
        if (currentScreen != null)
        {
            currentScreen.SetActive(false);
        }

        Debug.Log($"[TestNavigationManager] Navigating to screen: {targetScreen.name} from : {currentScreen.name}");

        //show target screen
        targetScreen.SetActive(true);
        currentScreen = targetScreen;
        screenHistory.Push(currentScreen);
    }

    public void GoBack()
    {
        string tempCuurent = "";
        string tempPrevious = "";
        if (screenHistory.Count > 1)
        {
            //pop current screen
            tempCuurent = screenHistory.Pop().name;
            //get previous screen
            GameObject previousScreen = screenHistory.Peek();

            tempPrevious = previousScreen.name;

            Debug.Log($"[TestNavigationManager] Going back from screen: {tempCuurent} to : {tempPrevious}");
            //hide current screen
            if (currentScreen != null)
            {
                currentScreen.SetActive(false);
            }
            //show previous screen
            previousScreen.SetActive(true);
            currentScreen = previousScreen;
        }
    }


}
