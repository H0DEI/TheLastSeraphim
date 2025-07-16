using DG.Tweening;
using UnityEngine;

public class FadeInElement : MonoBehaviour
{
    public float delay = 0f;
    public float duration = 0.5f;

    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        transform.localScale = Vector3.zero;
    }

    void Start()
    {
        transform.DOScale(1f, duration).SetDelay(delay).SetEase(Ease.OutBack);
        canvasGroup.DOFade(1f, duration).SetDelay(delay);
    }
}
