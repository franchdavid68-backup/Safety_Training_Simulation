using UnityEngine;
using DG.Tweening;

public class UIFadeIn : MonoBehaviour
{
    private CanvasGroup canvasGroup;


    [Header("Animation Settings")]
    [SerializeField] private float duration = 0.4f;

    void Awake()
    {
        
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }


    }

    void OnEnable()
    {


        canvasGroup.alpha = 0f;

        FadeIn();
    }

    public void FadeIn()
    {
        canvasGroup.DOKill();

        canvasGroup.DOFade(1f, duration);
    }
}