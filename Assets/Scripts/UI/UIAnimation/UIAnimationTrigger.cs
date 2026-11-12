using UnityEngine;

public class UIAnimationTrigger : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void PlayOnClickAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("PlayOnClick");
        }
    }
}