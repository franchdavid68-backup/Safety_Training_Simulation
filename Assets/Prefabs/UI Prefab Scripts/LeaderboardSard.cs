using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class LeaderboardSard : MonoBehaviour
{
    [SerializeField] private TMP_Text txtName;
    [SerializeField] private TMP_Text txtDuration;

    public void SetupRow(int place, string name, double duration)
    {
        if (name != null && duration != 0)
        {
           
            if (place == 1)
            {
                txtName.text = $"1st {name}";
                txtDuration.text = duration.ToString("F2");
            }
            else if (place == 2)
            {
                txtName.text = $"2nd {name}";
                txtDuration.text = duration.ToString("F2");
            }
            else if (place == 3)
            {
                txtName.text = $"3rd {name}";
                txtDuration.text = duration.ToString("F2");
            }
            else
            {
                txtName.text = name;
                txtDuration.text = duration.ToString("F2");
            }
        }
    }
}
