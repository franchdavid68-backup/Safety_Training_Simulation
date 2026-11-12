using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchUI : MonoBehaviour
{
    // Method to call when pressing start
    public void LoadGameScene()
    {
        SceneManager.LoadScene("Simulation");
    }

    //Method to call when pressing exit
    public void LoadUIScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("UI");
    }
}
