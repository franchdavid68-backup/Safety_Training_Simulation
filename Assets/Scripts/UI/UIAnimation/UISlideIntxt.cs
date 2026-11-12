using UnityEngine;
using DG.Tweening;

public class UISlideInTxt : MonoBehaviour
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private Vector2 defaultAnchoredPosition;
    private bool hasCachedPosition = false;

    [Header("Animation Settings")]
    [SerializeField] private float slideOffset = 50f; // How many pixels down it starts from
    [SerializeField] private float duration = 0.9f;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        if (!hasCachedPosition)
        {
            defaultAnchoredPosition = rectTransform.anchoredPosition;
            hasCachedPosition = true;
        }
    }

    void OnEnable()
    {
        if (hasCachedPosition)
        {
            rectTransform.anchoredPosition = defaultAnchoredPosition - new Vector2(0f, slideOffset);
        }

        canvasGroup.alpha = 0f;

        PlayAutoSlideIn();
    }

    public void PlayAutoSlideIn()
    {
        rectTransform.DOKill();
        canvasGroup.DOKill();

        rectTransform.DOAnchorPosY(defaultAnchoredPosition.y, duration)
                     .SetEase(Ease.OutQuad);

        canvasGroup.DOFade(1f, duration);
    }
}