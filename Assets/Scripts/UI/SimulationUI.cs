using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SimulationUI : MonoBehaviour
{
    [Header("Navigation")]

    public TestNavigationManager navigationManager;

    [Header("Button")]
    [SerializeField] private Button btnBack;
    [SerializeField] private Button btnSim;

    public void Start()
    {
        //basic onlick listeners
        if (btnBack != null)
        {
            btnBack.onClick.AddListener(() =>
            {
                navigationManager.GoBack();
            });
        }   
    }
}
